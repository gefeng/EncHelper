using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace EncHelper.Service
{
    [ServiceContract(Namespace = NSNames.NSENCHELPER1702, Name = "EncHelper")]
    public interface IEncryptionService
    {
        /// <summary>
        /// Bit wise xor calculation
        /// </summary>
        [OperationContract]
        string Xor(string blockA, string blockB);

        /// <summary>
        /// Calculate pin block based on given format
        /// </summary>
        [OperationContract]
        string CalPinBlock(string pan, string pin, PinBlockFormat format);

        /// <summary>
        /// Triple des encryption
        /// </summary>
        [OperationContract]
        string Encrypt3DES(string data, string key);

        /// <summary>
        /// Triple des decryption
        /// </summary>
        [OperationContract]
        string Decrypt3DES(string encData, string key);

        /// <summary>
        /// Calculate key check value
        /// </summary>
        [OperationContract]
        string CalKCV(string key);

        /// <summary>
        /// Calculate pvv
        /// </summary>
        [OperationContract]
        string CalPVV(string pan, string pin, string pvki, string pvk);

        /// <summary>
        /// Calculate cvv
        /// </summary>
        [OperationContract]
        string CalCVV(string pan, string expireDate, string serviceCode, string cvk);
    }

    [DataContract(Namespace = NSNames.NSENCHELPER1702, Name = "PinBlockFormat")]
    public enum PinBlockFormat
    {
        [EnumMember]
        ISO0,
        [EnumMember]
        VISA3,
    }
}
