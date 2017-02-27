using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EncHelper.Service
{
    public class EncryptionService : IEncryptionService
    {
        #region IEncryptionService

        // Bitwise xor, right-align  e.g. 1010 xor 001 => 1010 xor 0001 => 1011
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

        // Currently only support ECB(Electronic Codebook) mode
        // Key size must be 8 bytes (64 bits) or 16 Bytes (128 bits)
        // Data string must only contain hex digits and the length should be an even number
        public string Encrypt3DES(string data, string key)
        {
            byte[] dataInBytes;
            byte[] keyInBytes;
            byte[] encDataInBytes;

            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(key))
                return string.Empty;

            // Convert hex string to byte array
            dataInBytes = Enumerable.Range(0, data.Length / 2).Select(x => Convert.ToByte(data.Substring(x * 2, 2), 16)).ToArray();
            keyInBytes = Enumerable.Range(0, key.Length / 2).Select(x => Convert.ToByte(key.Substring(x * 2, 2), 16)).ToArray();

            if(keyInBytes.Length >= 16 && !TripleDES.IsWeakKey(keyInBytes))
            {
                TripleDESCryptoServiceProvider tDES = new TripleDESCryptoServiceProvider();
                tDES.Mode = CipherMode.ECB;
                tDES.Padding = PaddingMode.None;
                tDES.KeySize = 128;
                tDES.Key = keyInBytes;

                encDataInBytes = tDES.CreateEncryptor().TransformFinalBlock(dataInBytes, 0, dataInBytes.Length);
            }
            else
            {
                if (keyInBytes.Length > 8)
                    keyInBytes = keyInBytes.Take(8).ToArray();

                DESCryptoServiceProvider sDES = new DESCryptoServiceProvider();
                sDES.Mode = CipherMode.ECB;
                sDES.Padding = PaddingMode.None;
                sDES.KeySize = 64;
                sDES.Key = keyInBytes;

                encDataInBytes = sDES.CreateEncryptor().TransformFinalBlock(dataInBytes, 0, dataInBytes.Length);
            }

            return BitConverter.ToString(encDataInBytes).Replace("-", "");
        }

        // Currently only support ECB(Electronic Codebook) mode
        // Key size must be 8 bytes (64 bits) or 16 Bytes (128 bits)
        // Encrypted data string must only contain hex digits and the length should be an even number
        public string Decrypt3DES(string encData, string key)
        {
            byte[] encDataInBytes;
            byte[] keyInBytes;
            byte[] dataInBytes;

            if (string.IsNullOrEmpty(encData) || string.IsNullOrEmpty(key))
                return string.Empty;

            encDataInBytes = Enumerable.Range(0, encData.Length / 2).Select(x => Convert.ToByte(encData.Substring(x * 2, 2), 16)).ToArray();
            keyInBytes = Enumerable.Range(0, key.Length / 2).Select(x => Convert.ToByte(key.Substring(x * 2, 2), 16)).ToArray();

            if(keyInBytes.Length >= 16 && !TripleDES.IsWeakKey(keyInBytes))
            {
                TripleDESCryptoServiceProvider tDES = new TripleDESCryptoServiceProvider();
                tDES.Mode = CipherMode.ECB;
                tDES.Padding = PaddingMode.None;
                tDES.KeySize = 128;
                tDES.Key = keyInBytes;

                dataInBytes = tDES.CreateDecryptor().TransformFinalBlock(encDataInBytes, 0, encDataInBytes.Length);
            }
            else
            {
                if (keyInBytes.Length > 8)
                    keyInBytes = keyInBytes.Take(8).ToArray();

                DESCryptoServiceProvider sDES = new DESCryptoServiceProvider();
                sDES.Mode = CipherMode.ECB;
                sDES.Padding = PaddingMode.None;
                sDES.KeySize = 64;
                sDES.Key = keyInBytes;

                dataInBytes = sDES.CreateDecryptor().TransformFinalBlock(encDataInBytes, 0, encDataInBytes.Length);
            }

            return BitConverter.ToString(dataInBytes).Replace("-", "");
        }

        public string CalKCV(string key)
        {
            string retValue = string.Empty;
            string encBlock = string.Empty;

            if (string.IsNullOrEmpty(key))
                return retValue;

            encBlock = Encrypt3DES("0000000000000000", key);

            retValue = encBlock.Substring(0, 6);

            return retValue;
        }

        public string CalPVV(string pan, string pin, string pvki, string pvk)
        {
            string retValue = string.Empty;
            string encBlock = string.Empty;

            if (string.IsNullOrEmpty(pan) ||
                string.IsNullOrEmpty(pin) ||
                string.IsNullOrEmpty(pvki) ||
                string.IsNullOrEmpty(pvk))
            {
                return retValue;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(pan.Substring(pan.Length - 12, 11));
            sb.Append(pvki);
            sb.Append(pin);

            encBlock = Encrypt3DES(sb.ToString(), pvk);

            foreach(char hexChar in encBlock)
            {
                if(Helper.IsDigit(hexChar))
                {
                    retValue += hexChar;

                    if (retValue.Length == 4)
                        break;
                }
            }

            if(retValue.Length < 4)
            {
                foreach(char hexChar in encBlock)
                {
                    if(!Helper.IsDigit(hexChar))
                    {
                        retValue += (char.ToUpper(hexChar) - 'A');

                        if (retValue.Length == 4)
                            break;
                    }
                }
            }

            return retValue;
        }

        public string CalCVV(string pan, string expireDate, string serviceCode, string cvk)
        {
            string retValue = string.Empty;
            string blockA = string.Empty;
            string blockB = string.Empty;
            string encBlock = string.Empty;

            if (string.IsNullOrEmpty(pan) ||
                string.IsNullOrEmpty(expireDate) ||
                string.IsNullOrEmpty(serviceCode) ||
                string.IsNullOrEmpty(cvk))
            {
                return retValue;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(pan);
            sb.Append(expireDate);
            sb.Append(serviceCode);
            sb.Append('0', 32 - sb.Length);

            blockA = sb.ToString().Substring(0, 16);
            blockB = sb.ToString().Substring(16, 16);

            blockA = Encrypt3DES(blockA, cvk.Substring(0, 16));

            encBlock = Encrypt3DES(Xor(blockA, blockB), cvk);

            foreach (char hexChar in encBlock)
            {
                if (Helper.IsDigit(hexChar))
                {
                    retValue += hexChar;

                    if (retValue.Length == 3)
                        break;
                }
            }

            if (retValue.Length < 3)
            {
                foreach (char hexChar in encBlock)
                {
                    if (!Helper.IsDigit(hexChar))
                    {
                        retValue += (char.ToUpper(hexChar) - 'A');

                        if (retValue.Length == 3)
                            break;
                    }
                }
            }

            return retValue;
        }

        #endregion

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
