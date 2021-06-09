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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [TypeConverter(typeof(TypeCustomConverter<ILTagVersion>))]
    [JsonConverter(typeof(JsonCustomConverter<ILTagVersion>))]
    public class ILTagVersion : ILTagExplicit<Version>, ITextual<ILTagVersion>, IEquatable<ILTagVersion>
    {
        public ILTagVersion() : this(new Version()) {
        }

        public ILTagVersion(Version version) : base(ILTagId.Version, version) {
        }

        public ILTagVersion(string textualRepresentation) : base(ILTagId.Version, Version.Parse(textualRepresentation)) {
        }

        public override object AsJson => TextualRepresentation;

        public bool IsEmpty => Value is null;

        public bool IsInvalid => false;
        public string TextualRepresentation => Value.ToString();

        public static ILTagVersion FromJson(object o) => new(new Version((string)o));

        public static bool operator !=(ILTagVersion left, ILTagVersion right) => !(left == right);

        public static bool operator ==(ILTagVersion left, ILTagVersion right) => EqualityComparer<ILTagVersion>.Default.Equals(left, right);

        public override bool Equals(object obj) => Equals(obj as ILTagVersion);

        public bool Equals(ILTagVersion other) => other != null && TextualRepresentation == other.TextualRepresentation;

        public override int GetHashCode() => HashCode.Combine(TextualRepresentation);

        internal ILTagVersion(Stream s) : base(ILTagId.Version, s) {
        }

        protected override Version FromBytes(byte[] bytes) {
            return FromBytesHelper(bytes, s => Build(s.BigEndianReadInt(), s.BigEndianReadInt(), s.BigEndianReadInt(), s.BigEndianReadInt()));

            static Version Build(int major, int minor, int build, int revision)
                => revision >= 0
                    ? new Version(major, minor, build, revision)
                    : build >= 0
                        ? new Version(major, minor, build)
                        : new Version(major, minor);
        }

        protected override byte[] ToBytes(Version value)
            => TagHelpers.ToBytesHelper(s => s.BigEndianWriteInt(value.Major).BigEndianWriteInt(value.Minor).BigEndianWriteInt(value.Build).BigEndianWriteInt(value.Revision));
    }
}