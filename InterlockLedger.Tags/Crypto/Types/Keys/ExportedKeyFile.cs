/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/


namespace InterlockLedger.Tags
{
    public class ExportedKeyFile
    {
        public readonly string KeyName;
        public readonly string KeyFileName;
        public readonly byte[] KeyFileBytes;

        public ExportedKeyFile(string keyName, string keyFileName, byte[] keyFileBytes) {
            if (string.IsNullOrWhiteSpace(keyName))
                throw new System.ArgumentException("Can't have an empty key name!!!", nameof(keyName));
            if (string.IsNullOrWhiteSpace(keyFileName))
                throw new System.ArgumentException("Can't have an empty key filename!!!", nameof(keyFileName));
            if (keyFileBytes == null || keyFileBytes.Length == 0)
                throw new System.ArgumentException("Can't have zero key file bytes!!!", nameof(keyFileBytes));
            KeyName = keyName;
            KeyFileName = keyFileName;
            KeyFileBytes = keyFileBytes;
        }
    }
}