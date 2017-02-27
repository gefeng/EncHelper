using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncHelper
{
    internal static class Helper
    {
        public static void AlignToRight(int length, ref string hexString)
        {
            if (length <= 0)
                return;

            for (int i = 0; i < length; ++i)
            {
                hexString = hexString.Insert(0, "0");
            }
        }

        public static int HexCharToInt(char hexChar)
        {
            if (hexChar >= '0' && hexChar <= '9')
            {
                return (int)(hexChar - '0');
            }

            char upper = char.ToUpper(hexChar);

            if(upper >= 'A' && upper <= 'F')
            {
                return 10 + (int)(upper - 'A');
            }

            throw new ArgumentOutOfRangeException("hexChar");
        }

        public static bool IsDigit(char inputChar)
        {
            if (inputChar >= '0' && inputChar <= '9')
            {
                return true;
            }

            return false;
        }
    }
}
