/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;

namespace InterlockLedger.Tags
{
    public class EncryptedBlob : EncryptedValue<ILTagByteArray>
    {
        public static readonly DataField FieldDefinition = new EncryptedBlob().FieldModel;

        public EncryptedBlob() {
        }

        public EncryptedBlob(CipherAlgorithm cipher, byte[] blobInClearText, ISigner author, IEnumerable<TagReader> readers)
            : base(cipher, new ILTagByteArray(blobInClearText), author, readers) {
        }

        protected override ulong TagId => ILTagId.EncryptedBlob;
        protected override string TypeName => nameof(EncryptedBlob);
    }
}