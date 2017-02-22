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
            string pinBlock = string.Empty;

            if (string.IsNullOrEmpty(pin))
            {
                return pinBlock;
            }

            if(pin.Length < 4 || pin.Length > 12)
            {
                return pinBlock;
            }

            if (format == PinBlockFormat.ISO0)
            {
                pinBlock = CalPinBlockInFormatISO0(pan, pin);
            }
            else if(format == PinBlockFormat.VISA3)
            {
                pinBlock = CalPinBlockInFormatVISA3(pin);
            }

            return pinBlock;
        }

        private string CalPinBlockInFormatISO0(string pan, string pin)
        {
            StringBuilder sbPin = new StringBuilder("0");
            StringBuilder sbPan = new StringBuilder("0000");

            sbPin.Append(pin.Length.ToString() + pin);
            sbPin.Append('F', 14 - pin.Length);

            sbPan.Append(pan.Substring(pan.Length - 13, 12));

            return Xor(sbPin.ToString(), sbPan.ToString());
        }

        private string CalPinBlockInFormatVISA3(string pin)
        {
            StringBuilder sb = new StringBuilder(pin);
                
            sb.Append('F', 16 - pin.Length);

            return sb.ToString();
        }
    }
}
