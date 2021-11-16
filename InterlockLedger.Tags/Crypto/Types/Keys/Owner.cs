// ******************************************************************************************************************************
//
// Copyright (c) 2018-2021 InterlockLedger Network
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
public abstract class Owner : ISigningKey, IPasswordProvider
{
    public InterlockKey AsInterlockKey => new(this);
    public string Description { get; protected set; }
    public string Email { get; protected set; }
    public BaseKeyId Id { get; protected set; }
    public string Name { get; protected set; }
    public OwnerId OwnerId => (OwnerId)Id;
    public IEnumerable<AppPermissions> Permissions { get; } = InterlockKey.Parts.NoPermissions;
    public TagPubKey PublicKey { get; protected set; }
    public KeyPurpose[] Purposes => keyPurposes;
    public Algorithm SignAlgorithm { get; protected set; }
    public KeyStrength Strength { get; protected set; }

    public abstract byte[] Decrypt(byte[] bytes);

    public string PasswordFor(InterlockId id) => Convert.ToBase64String(Sign(id.Required().EncodedBytes).Data);

    public abstract TagSignature Sign(byte[] data);

    public abstract TagSignature Sign<T>(T data) where T : Signable<T>, new();

    public string ToListing() => $"'{Name}' {Id}";

    public string ToShortString() => $"Owner {Name} using {SignAlgorithm} with {Strength} strength ({Id})";

    public override string ToString() => ToShortString() + $"\r\n-- {Description}\r\n-- Email {Email}\r\n-- {PublicKey}\r\n-- Purposes {keyPurposes.ToStringAsList()}";

    protected static readonly KeyPurpose[] keyPurposes = new KeyPurpose[] { KeyPurpose.ChainOperation, KeyPurpose.Protocol };
}