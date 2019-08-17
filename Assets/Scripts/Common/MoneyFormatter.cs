using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD
{
    static class MoneyFormatter
    {
        public const char MoneySymbol = '$'; // ⊙

        public static string MoneyFormat { get; } = string.Concat(MoneySymbol, " {0}");

        public static string FormatMoney(int amount) { return string.Format(MoneyFormat, amount); }
    }
}
