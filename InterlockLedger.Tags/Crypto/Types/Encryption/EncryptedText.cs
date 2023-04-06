// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2023 InterlockLedger Network
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met
//
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
//
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// * Neither the name of the copyright holder nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES, LOSS OF USE, DATA, OR PROFITS, OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// ******************************************************************************************************************************

namespace InterlockLedger.Tags;
public class EncryptedText : VersionedValue<EncryptedText>
{
    public static readonly DataField FieldDefinition = new EncryptedText().FieldModel;

    public EncryptedText() : base(ILTagId.EncryptedText, EncryptedValue<ILTagString>.CurrentVersion)
        => _encrypted = new EncryptedValue<ILTagString>(ILTagId.EncryptedText);

    public EncryptedText(CipherAlgorithm cipher, string clearText, IEncryptor encryptor, IIdentifiedPublicKey author, IEnumerable<TagReader> readers) : this()
        => _encrypted = new EncryptedValue<ILTagString>(ILTagId.EncryptedText, cipher, encryptor, new ILTagString(clearText), author, readers);

    public CipherAlgorithm Cipher => _encrypted.Cipher;
    public byte[] CipherText => _encrypted.CipherText;
    [JsonIgnore]
    public override string Formatted => $"Encrypted using {Cipher} with {CipherText.Length} bytes";
    public IEnumerable<TagReadingKey> ReadingKeys => _encrypted.ReadingKeys;
    public override string TypeName => nameof(EncryptedText);

    public static EncryptedText Embed(EncryptedValue<ILTagString> value)
        => new(value.Required());

    public ILTagString Decrypt(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) => _encrypted.Decrypt(reader, findEngine);

    public string DecryptText(IReader reader, Func<CipherAlgorithm, ISymmetricEngine> findEngine) => Decrypt(reader, findEngine).Value;

    public override EncryptedText FromJson(object json) => new(_encrypted.FromJson(json));

    protected override IEnumerable<DataField> RemainingStateFields => _encrypted.RemainingStateFields;

    protected override string TypeDescription => "A text encrypted for some readers";

    protected override void DecodeRemainingStateFrom(Stream s) => _encrypted.DecodeRemainingStateFrom(s);

    protected override void EncodeRemainingStateTo(Stream s) => _encrypted.EncodeRemainingStateTo(s);

    private readonly EncryptedValue<ILTagString> _encrypted;

    private EncryptedText(EncryptedValue<ILTagString> encrypted) : base(ILTagId.EncryptedText, encrypted.Version) => _encrypted = encrypted;
}