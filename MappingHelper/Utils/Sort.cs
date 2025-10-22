using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MappingHelper
{
    public static class NaturalSortExtensions
    {
        /// <summary>
        /// 对字符串数组进行自然排序（类似Windows资源管理器的排序方式）
        /// </summary>
        /// <param name="fileNames">要排序的文件名数组</param>
        /// <returns>按自然顺序排序后的数组</returns>
        public static string[] NaturalSort(this string[] fileNames)
        {
            return fileNames.OrderBy(x => x, new NaturalStringComparer()).ToArray();
        }

        /// <summary>
        /// 对字符串列表进行自然排序
        /// </summary>
        /// <param name="fileNames">要排序的文件名列表</param>
        /// <returns>按自然顺序排序后的列表</returns>
        public static List<string> NaturalSort(this List<string> fileNames)
        {
            return fileNames.OrderBy(x => x, new NaturalStringComparer()).ToList();
        }
    }

    /// <summary>
    /// 自然字符串比较器
    /// </summary>
    public class NaturalStringComparer : IComparer<string>
    {
        private static readonly Regex _regex = new Regex(@"(\d+)", RegexOptions.Compiled);

        public int Compare(string x, string y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var xParts = SplitStringIntoParts(x);
            var yParts = SplitStringIntoParts(y);

            int minLength = Math.Min(xParts.Count, yParts.Count);

            for (int i = 0; i < minLength; i++)
            {
                var xPart = xParts[i];
                var yPart = yParts[i];

                // 如果两个部分都是数字，按数值比较
                if (IsNumeric(xPart.Value) && IsNumeric(yPart.Value))
                {
                    long xNum = long.Parse(xPart.Value);
                    long yNum = long.Parse(yPart.Value);

                    int numComparison = xNum.CompareTo(yNum);
                    if (numComparison != 0)
                        return numComparison;
                }
                else
                {
                    // 按字符串比较（不区分大小写）
                    int stringComparison = string.Compare(xPart.Value, yPart.Value,
                        StringComparison.OrdinalIgnoreCase);
                    if (stringComparison != 0)
                        return stringComparison;
                }
            }

            // 如果前面的部分都相等，比较长度
            return xParts.Count.CompareTo(yParts.Count);
        }

        /// <summary>
        /// 将字符串分解为数字和非数字部分
        /// </summary>
        private List<StringPart> SplitStringIntoParts(string input)
        {
            var parts = new List<StringPart>();
            var matches = _regex.Matches(input);

            int lastIndex = 0;

            foreach (Match match in matches)
            {
                // 添加数字前的文本部分
                if (match.Index > lastIndex)
                {
                    string textPart = input.Substring(lastIndex, match.Index - lastIndex);
                    parts.Add(new StringPart { Value = textPart, IsNumeric = false });
                }

                // 添加数字部分
                parts.Add(new StringPart { Value = match.Value, IsNumeric = true });
                lastIndex = match.Index + match.Length;
            }

            // 添加最后剩余的文本部分
            if (lastIndex < input.Length)
            {
                string textPart = input.Substring(lastIndex);
                parts.Add(new StringPart { Value = textPart, IsNumeric = false });
            }

            return parts;
        }

        /// <summary>
        /// 检查字符串是否为数字
        /// </summary>
        private bool IsNumeric(string value)
        {
            return long.TryParse(value, out _);
        }

        /// <summary>
        /// 字符串部分类，用于标记是否为数字
        /// </summary>
        private class StringPart
        {
            public string Value { get; set; }
            public bool IsNumeric { get; set; }
        }
    }
}