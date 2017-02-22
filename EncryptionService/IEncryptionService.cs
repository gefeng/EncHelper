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
