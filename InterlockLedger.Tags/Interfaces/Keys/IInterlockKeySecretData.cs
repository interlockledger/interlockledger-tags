/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IInterlockKeySecretData
    {
        byte[] Encrypted { get; }
        EncryptedContentType EncryptedContentType { get; }

        string ToShortString();
    }
}
