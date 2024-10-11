using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Open.Tools.Utils
{
    public static class CurrencyUtils
    {
        /// <summary>
        /// 根据本地语言格式化货币价格
        /// </summary>
        /// <param name="money">价格</param>
        /// <param name="decimalBit">保留几位小数</param>
        /// <param name="currencySymbol">货币符号</param>
        /// <returns>货币价格(例如：HK$12.09)</returns>
        public static string FormatMoney(decimal money, int decimalBit, string currencySymbol)
        {
            var culture = new CultureInfo(CultureInfo.CurrentCulture.Name)
            {
                NumberFormat =
                {
                    CurrencySymbol = currencySymbol
                }
            };

            return money.ToString($"C{decimalBit}", culture);
        }

        /// <summary>
        /// 根据格式化的价格截取货币符号
        /// 思路：去掉字符串里面的数字、小数点（.）、逗号（,）空格
        /// </summary>
        /// <param name="formatPrice"></param>
        /// <returns>货币符号</returns>
        public static string GetCurrencySymbolForFormatPrice(string formatPrice)
        {
            if (string.IsNullOrEmpty(formatPrice))
            {
                return formatPrice;
            }

            var pattern = @"[^\d., ]+";
            var matches = Regex.Matches(formatPrice, pattern);
            var result = "";
            foreach (Match match in matches)
            {
                result += match.Value;
            }

            return result;
        }
    }
}