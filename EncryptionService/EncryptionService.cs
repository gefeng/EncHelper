using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncHelper.EncryptionService
{
    public class EncryptionService : IEncryptionService
    {
        public string Xor(string blockA, string blockB)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(blockA) || string.IsNullOrEmpty(blockB))
                return result;

            foreach (char hexChar in blockA)
            {
                int hexInt = Helper.HexCharToInt(hexChar);
            }

            return result;
        }

        public string CalPinBlock(string pan, string pin, PinBlockFormat format)
        {
            return "";
        }
    }
}
