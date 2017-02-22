using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncHelper.Service
{
    public class EncryptionService : IEncryptionService
    {
        // Bitwise xor, align to right  e.g. 1010 xor 001 => 1010 xor 0001 => 1011
        public string Xor(string blockA, string blockB)
        {
            StringBuilder sb = new StringBuilder();

            int blockLength = (blockA.Length > blockB.Length) ? blockA.Length : blockB.Length;

            Helper.AlignToRight((blockLength-blockA.Length), ref blockA);
            Helper.AlignToRight((blockLength-blockB.Length), ref blockB);

            for (int i = 0; i < blockLength; ++i)
            {
                sb.Append((Helper.HexCharToInt(blockA[i]) ^ Helper.HexCharToInt(blockB[i])).ToString("X"));
            }

            return sb.ToString();
        }

        public string CalPinBlock(string pan, string pin, PinBlockFormat format)
        {
            return "";
        }
    }
}
