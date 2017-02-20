using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncHelper
{
    internal static class Helper
    {
        public static int HexCharToInt(char hexChar)
        {
            if (hexChar >= '0' && hexChar <= '9')
            {
                return (int)(hexChar - '0');
            }

            char upper = char.ToUpper(hexChar);

            if(hexChar >= 'A' && hexChar <= 'F')
            {
                return 10 + (int)(hexChar - 'A');
            }

            throw new ArgumentOutOfRangeException("hexChar");
        }
    }
}
