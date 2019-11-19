/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface ISigner
    {
        Algorithm Algorithm { get; }
        InterlockKey AsInterlockKey { get; }
        BaseKeyId Id { get; }
        string Name { get; }
        TagPubKey PublicKey { get; }
        KeyStrength Strength { get; }

        (byte[] cypherText, byte[] key, byte[] iv) Encrypt<T>(CipherAlgorithm cipher, T clearText) where T : ILTag;

        (byte[] cypherText, byte[] key, byte[] iv) EncryptRaw(CipherAlgorithm cipher, byte[] clearText);

        TagSignature Sign(byte[] data, KeyPurpose purpose, ulong? appId = null, Algorithm algorithm = Algorithm.RSA);

        bool Supports(KeyPurpose purpose, ulong? appId = null);
    }
}