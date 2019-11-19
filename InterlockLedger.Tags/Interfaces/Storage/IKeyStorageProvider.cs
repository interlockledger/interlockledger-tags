/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using InterlockLedger.Tags;

namespace InterlockLedger.Tags
{
    public interface IKeyStorageProvider : IKeyFileExporter
    {
        IEnumerable<InterlockSigningKeyData> Keys { get; }
        string AppKeysFolderPath { get; }

        InterlockSigningKeyData Create(KeyPurpose[] purposes, ulong appId, IEnumerable<ulong> actionIds, Algorithm algorithm, KeyStrength strength, string name, string description, string password);

        InterlockSigningKeyData Import(KeyPurpose[] purposes, ulong appId, IEnumerable<ulong> actionIds, byte[] certificateBytes, string password);

        InterlockSigningKey Open(InterlockSigningKeyData key, string password);

        InterlockSigningKey Resolve(string name, string password);
    }
}
