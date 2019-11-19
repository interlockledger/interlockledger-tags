/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

namespace InterlockLedger.Tags
{
    public interface IUpdatingKeyStorageProvider
    {
        bool FileExists { get; }
        BaseKeyId OwnerId { get; }
        InterlockUpdatableSigningKey Resolved { get; }

        InterlockUpdatableSigningKeyData Create();

        byte[] Encrypt(byte[] secret);

        void Save(InterlockUpdatableSigningKey key);
    }
}