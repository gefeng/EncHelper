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
        [OperationContract]
        string Xor(string blockA, string blockB);

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
