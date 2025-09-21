using System;

namespace MappingHelper
{
    using System;

    /// <summary>
    /// DOTween 缓动类型枚举
    /// </summary>
    public enum EaseType
    {
        Linear,
        InSine, OutSine, InOutSine,
        InQuad, OutQuad, InOutQuad,
        InCubic, OutCubic, InOutCubic,
        InQuart, OutQuart, InOutQuart,
        InQuint, OutQuint, InOutQuint,
        InExpo, OutExpo, InOutExpo,
        InCirc, OutCirc, InOutCirc,
        InElastic, OutElastic, InOutElastic,
        InBack, OutBack, InOutBack,
        InBounce, OutBounce, InOutBounce,
        Flash, InFlash, OutFlash, InOutFlash
    }

    /// <summary>
    /// DOTween 缓动函数静态工具类
    /// </summary>
    public static class DOTweenEaseUtils
    {
        private const float PI = (float)Math.PI;
        private const float HALF_PI = PI / 2f;
        private const float TWO_PI = PI * 2f;

        /// <summary>
        /// 根据缓动类型计算对应的值
        /// </summary>
        /// <param name="t">时间参数 (0-1)</param>
        /// <param name="easeType">缓动类型</param>
        /// <returns>缓动后的值 (0-1)</returns>
        public static float Evaluate(float t, EaseType easeType)
        {
            // 确保 t 在 [0, 1] 范围内
            t = Math.Max(0f, Math.Min(1f, t));

            switch (easeType)
            {
                case EaseType.Linear:
                    return Linear(t);

                case EaseType.InSine:
                    return InSine(t);
                case EaseType.OutSine:
                    return OutSine(t);
                case EaseType.InOutSine:
                    return InOutSine(t);

                case EaseType.InQuad:
                    return InQuad(t);
                case EaseType.OutQuad:
                    return OutQuad(t);
                case EaseType.InOutQuad:
                    return InOutQuad(t);

                case EaseType.InCubic:
                    return InCubic(t);
                case EaseType.OutCubic:
                    return OutCubic(t);
                case EaseType.InOutCubic:
                    return InOutCubic(t);

                case EaseType.InQuart:
                    return InQuart(t);
                case EaseType.OutQuart:
                    return OutQuart(t);
                case EaseType.InOutQuart:
                    return InOutQuart(t);

                case EaseType.InQuint:
                    return InQuint(t);
                case EaseType.OutQuint:
                    return OutQuint(t);
                case EaseType.InOutQuint:
                    return InOutQuint(t);

                case EaseType.InExpo:
                    return InExpo(t);
                case EaseType.OutExpo:
                    return OutExpo(t);
                case EaseType.InOutExpo:
                    return InOutExpo(t);

                case EaseType.InCirc:
                    return InCirc(t);
                case EaseType.OutCirc:
                    return OutCirc(t);
                case EaseType.InOutCirc:
                    return InOutCirc(t);

                case EaseType.InElastic:
                    return InElastic(t);
                case EaseType.OutElastic:
                    return OutElastic(t);
                case EaseType.InOutElastic:
                    return InOutElastic(t);

                case EaseType.InBack:
                    return InBack(t);
                case EaseType.OutBack:
                    return OutBack(t);
                case EaseType.InOutBack:
                    return InOutBack(t);

                case EaseType.InBounce:
                    return InBounce(t);
                case EaseType.OutBounce:
                    return OutBounce(t);
                case EaseType.InOutBounce:
                    return InOutBounce(t);

                case EaseType.Flash:
                    return Flash(t);
                case EaseType.InFlash:
                    return InFlash(t);
                case EaseType.OutFlash:
                    return OutFlash(t);
                case EaseType.InOutFlash:
                    return InOutFlash(t);

                default:
                    return Linear(t);
            }
        }

        #region Linear
        private static float Linear(float t) => t;
        #endregion

        #region Sine
        private static float InSine(float t) => 1f - (float)Math.Cos(t * HALF_PI);
        private static float OutSine(float t) => (float)Math.Sin(t * HALF_PI);
        private static float InOutSine(float t) => -(float)Math.Cos(PI * t) / 2f + 0.5f;
        #endregion

        #region Quad
        private static float InQuad(float t) => t * t;
        private static float OutQuad(float t) => 1f - (1f - t) * (1f - t);
        private static float InOutQuad(float t) => t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2f) / 2f;
        #endregion

        #region Cubic
        private static float InCubic(float t) => t * t * t;
        private static float OutCubic(float t) => 1f - (float)Math.Pow(1f - t, 3f);
        private static float InOutCubic(float t) => t < 0.5f ? 4f * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 3f) / 2f;
        #endregion

        #region Quart
        private static float InQuart(float t) => t * t * t * t;
        private static float OutQuart(float t) => 1f - (float)Math.Pow(1f - t, 4f);
        private static float InOutQuart(float t) => t < 0.5f ? 8f * t * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 4f) / 2f;
        #endregion

        #region Quint
        private static float InQuint(float t) => t * t * t * t * t;
        private static float OutQuint(float t) => 1f - (float)Math.Pow(1f - t, 5f);
        private static float InOutQuint(float t) => t < 0.5f ? 16f * t * t * t * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 5f) / 2f;
        #endregion

        #region Expo
        private static float InExpo(float t) => t == 0f ? 0f : (float)Math.Pow(2f, 10f * (t - 1f));
        private static float OutExpo(float t) => t == 1f ? 1f : 1f - (float)Math.Pow(2f, -10f * t);
        private static float InOutExpo(float t)
        {
            if (t == 0f) return 0f;
            if (t == 1f) return 1f;
            return t < 0.5f ? (float)Math.Pow(2f, 20f * t - 10f) / 2f : (2f - (float)Math.Pow(2f, -20f * t + 10f)) / 2f;
        }
        #endregion

        #region Circ
        private static float InCirc(float t) => 1f - (float)Math.Sqrt(1f - t * t);
        private static float OutCirc(float t) => (float)Math.Sqrt(1f - (t - 1f) * (t - 1f));
        private static float InOutCirc(float t)
        {
            return t < 0.5f ?
                (1f - (float)Math.Sqrt(1f - (2f * t) * (2f * t))) / 2f :
                ((float)Math.Sqrt(1f - (-2f * t + 2f) * (-2f * t + 2f)) + 1f) / 2f;
        }
        #endregion

        #region Elastic
        private static float InElastic(float t)
        {
            const float c4 = TWO_PI / 3f;
            return t == 0f ? 0f : t == 1f ? 1f : -(float)Math.Pow(2f, 10f * t - 10f) * (float)Math.Sin((t * 10f - 10.75f) * c4);
        }

        private static float OutElastic(float t)
        {
            const float c4 = TWO_PI / 3f;
            return t == 0f ? 0f : t == 1f ? 1f : (float)Math.Pow(2f, -10f * t) * (float)Math.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        private static float InOutElastic(float t)
        {
            const float c5 = TWO_PI / 4.5f;
            return t == 0f ? 0f : t == 1f ? 1f : t < 0.5f ?
                -((float)Math.Pow(2f, 20f * t - 10f) * (float)Math.Sin((20f * t - 11.125f) * c5)) / 2f :
                ((float)Math.Pow(2f, -20f * t + 10f) * (float)Math.Sin((20f * t - 11.125f) * c5)) / 2f + 1f;
        }
        #endregion

        #region Back
        private static float InBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return c3 * t * t * t - c1 * t * t;
        }

        private static float OutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * (float)Math.Pow(t - 1f, 3f) + c1 * (float)Math.Pow(t - 1f, 2f);
        }

        private static float InOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return t < 0.5f ?
                ((float)Math.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) / 2f :
                ((float)Math.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) / 2f;
        }
        #endregion

        #region Bounce
        private static float OutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (t < 1f / d1)
                return n1 * t * t;
            else if (t < 2f / d1)
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            else if (t < 2.5f / d1)
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            else
                return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }

        private static float InBounce(float t) => 1f - OutBounce(1f - t);

        private static float InOutBounce(float t) => t < 0.5f ?
            (1f - OutBounce(1f - 2f * t)) / 2f :
            (1f + OutBounce(2f * t - 1f)) / 2f;
        #endregion

        #region Flash
        private static float Flash(float t) => (float)Math.Sin(PI * t * 3f) * (1f - t);

        private static float InFlash(float t)
        {
            return (float)Math.Sin(PI * t * t * 5f) * t;
        }

        private static float OutFlash(float t)
        {
            float invT = 1f - t;
            return 1f - (float)Math.Sin(PI * invT * invT * 5f) * invT;
        }

        private static float InOutFlash(float t)
        {
            if (t < 0.5f)
            {
                float t2 = t * 2f;
                return (float)Math.Sin(PI * t2 * t2 * 5f) * t2 * 0.5f;
            }
            else
            {
                float invT = (1f - t) * 2f;
                return 1f - (float)Math.Sin(PI * invT * invT * 5f) * invT * 0.5f;
            }
        }
        #endregion

        #region 便利方法

        /// <summary>
        /// 在指定范围内进行缓动插值
        /// </summary>
        /// <param name="t">时间参数 (0-1)</param>
        /// <param name="from">起始值</param>
        /// <param name="to">目标值</param>
        /// <param name="easeType">缓动类型</param>
        /// <returns>插值结果</returns>
        public static float Lerp(float t, float from, float to, EaseType easeType)
        {
            float easedT = Evaluate(t, easeType);
            return from + (to - from) * easedT;
        }

        /// <summary>
        /// 获取所有可用的缓动类型
        /// </summary>
        /// <returns>缓动类型数组</returns>
        public static EaseType[] GetAllEaseTypes()
        {
            return (EaseType[])Enum.GetValues(typeof(EaseType));
        }

        #endregion
    }
}