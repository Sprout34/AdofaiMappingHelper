using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

public static class TextToPNG
{
    public static void GeneratePNG(
        string text,
        string outputPath,
        string fontPath = null,
        float fontSize = 80,

        // 描边
        bool enableStroke = false,
        int strokeSize = 2,
        Color? strokeColor = null,

        // 阴影
        bool enableShadow = false,
        Tuple<float, float> shadowOffset = null,
        int shadowBlurRadius = 15,
        float shadowDensity = 1f,   // 范围 0.1 ~ 2，<1 扩散，>1 浓实
        Color? shadowColor = null,

        //文字颜色
        Color? color = null
    )
    {
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        // shadowDensity 安全限制，防止除零或负数导致曲线异常
        shadowDensity = Clamp(shadowDensity, 0.1f, 2f);

        Font font;
        if (!string.IsNullOrEmpty(fontPath) && File.Exists(fontPath))
        {
            PrivateFontCollection fonts = new PrivateFontCollection();
            fonts.AddFontFile(fontPath);
            font = new Font(fonts.Families[0], fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
        }
        else
        {
            font = new Font(FontFamily.GenericSansSerif, fontSize, GraphicsUnit.Pixel);
        }

        SizeF textSize;
        using (Bitmap tmp = new Bitmap(1, 1))
        using (Graphics g = Graphics.FromImage(tmp))
        {
            textSize = g.MeasureString(text, font);
        }

        int padding = (int)(fontSize + shadowBlurRadius * 2 + 20);
        int width = (int)textSize.Width + padding * 2;
        int height = (int)textSize.Height + padding * 2;

        using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
        using (Graphics g2 = Graphics.FromImage(bmp))
        {
            g2.SmoothingMode = SmoothingMode.AntiAlias;
            g2.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            GraphicsPath path = new GraphicsPath();
            path.AddString(
                text,
                font.FontFamily,
                (int)FontStyle.Regular,
                fontSize,
                new Point(padding, padding),
                StringFormat.GenericDefault
            );

            // ① 阴影
            if (enableShadow)
            {
                float offX = shadowOffset != null ? shadowOffset.Item1 : fontSize * 0.1f;
                float offY = shadowOffset != null ? shadowOffset.Item2 : fontSize * 0.1f;
                Color shColor = shadowColor ?? Color.Black;

                using (Bitmap shadowLayer = RenderShadowLayer(
                    path, width, height, shColor, offX, offY, shadowBlurRadius, shadowDensity))
                {
                    g2.DrawImage(shadowLayer, 0, 0);
                }
            }

            // ② 描边
            if (enableStroke)
            {
                using (Pen pen = new Pen(strokeColor ?? Color.Black, strokeSize))
                {
                    pen.LineJoin = LineJoin.Round;
                    g2.DrawPath(pen, path);
                }
            }

            // ③ 主文字
            using (SolidBrush brush = new SolidBrush(color ?? Color.White))
            {
                g2.FillPath(brush, path);
            }

            bmp.Save(outputPath, ImageFormat.Png);
        }
    }

    private static Bitmap RenderShadowLayer(
        GraphicsPath path, int w, int h,
        Color shadowColor,
        float offsetX, float offsetY,
        int blurRadius,
        float density)
    {
        // 第一步：把路径画到临时图层，仅用于提取 alpha 轮廓
        byte[] alpha = new byte[w * h];

        using (Bitmap maskBmp = new Bitmap(w, h, PixelFormat.Format32bppArgb))
        {
            using (Graphics g = Graphics.FromImage(maskBmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                GraphicsPath shifted = (GraphicsPath)path.Clone();
                Matrix m = new Matrix();
                m.Translate(offsetX, offsetY);
                shifted.Transform(m);

                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    g.FillPath(brush, shifted);
                }
                shifted.Dispose();
            }

            BitmapData maskData = maskBmp.LockBits(
                new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            int stride = maskData.Stride;
            byte[] buf = new byte[stride * h];
            Marshal.Copy(maskData.Scan0, buf, 0, buf.Length);
            maskBmp.UnlockBits(maskData);

            // 只取 alpha 通道（字节序 B G R A，偏移 +3）
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    alpha[y * w + x] = buf[y * stride + x * 4 + 3];
                }
        }

        // 第二步：仅对 alpha 数组做高斯模糊（水平 + 垂直两遍）
        float[] kernel = BuildGaussianKernel(blurRadius);
        float[] srcFloat = new float[w * h];
        float[] tmpFloat = new float[w * h];

        for (int i = 0; i < alpha.Length; i++)
            srcFloat[i] = alpha[i];

        // 水平方向
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float sum = 0;
                for (int k = -blurRadius; k <= blurRadius; k++)
                {
                    int sx = Clamp(x + k, 0, w - 1);
                    sum += srcFloat[y * w + sx] * kernel[k + blurRadius];
                }
                tmpFloat[y * w + x] = sum;
            }

        // 垂直方向
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float sum = 0;
                for (int k = -blurRadius; k <= blurRadius; k++)
                {
                    int sy = Clamp(y + k, 0, h - 1);
                    sum += tmpFloat[sy * w + x] * kernel[k + blurRadius];
                }
                srcFloat[y * w + x] = sum;
            }

        // 第三步：应用 density 曲线后合并 shadowColor 输出
        Bitmap result = new Bitmap(w, h, PixelFormat.Format32bppArgb);
        BitmapData resData = result.LockBits(
            new Rectangle(0, 0, w, h),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppArgb);

        int resStride = resData.Stride;
        byte[] resBuf = new byte[resStride * h];
        float colorAlphaFactor = shadowColor.A / 255f;

        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float normalized = srcFloat[y * w + x] / 255f;

                // 幂次曲线：density>1 更浓实，density<1 更扩散
                float curved = (float)Math.Pow(normalized, 1f / density);

                int di = y * resStride + x * 4;
                resBuf[di] = shadowColor.B;
                resBuf[di + 1] = shadowColor.G;
                resBuf[di + 2] = shadowColor.R;
                resBuf[di + 3] = (byte)Clamp((int)(curved * 255f * colorAlphaFactor), 0, 255);
            }

        Marshal.Copy(resBuf, 0, resData.Scan0, resBuf.Length);
        result.UnlockBits(resData);

        return result;
    }

    private static float[] BuildGaussianKernel(int radius)
    {
        int size = radius * 2 + 1;
        float sigma = radius / 3.0f;
        float sigma2 = 2 * sigma * sigma;
        float[] kernel = new float[size];
        float sum = 0;

        for (int i = 0; i < size; i++)
        {
            float x = i - radius;
            kernel[i] = (float)Math.Exp(-(x * x) / sigma2);
            sum += kernel[i];
        }
        for (int i = 0; i < size; i++)
            kernel[i] /= sum;

        return kernel;
    }

    private static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    private static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}