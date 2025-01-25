// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2025 InterlockLedger Network
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

public class ILTagDataModel : ILTagOfExplicit<DataModel>
{
    public ILTagDataModel(DataModel model) : base(ILTagId.DataModel, model) {
    }

    public ILTagDataModel(Stream s) : base(ILTagId.DataModel, s) {
    }

    protected override Task<DataModel?> ValueFromStreamAsync(WrappedReadonlyStream s) {
        var payloadTagId = s.DecodeILInt();
        s.DecodeILInt(); // drop deprecated field
        var result = new DataModel {
            PayloadTagId = payloadTagId,
            DataFields = s.DecodeTagArray<ILTagDataField>().Safe().Select(t => t.Required().Value.Required()),
            Indexes = s.DecodeTagArray<ILTagDataIndex>().Safe().Select(t => t.Required().Value.Required()).NullIfEmpty(),
            PayloadName = s.DecodeString(),
            Version = s.HasBytes() ? s.DecodeUShort() : (ushort)1,
            Description = s.HasBytes() ? s.DecodeString().TrimToNull() : null
        };
        return Task.FromResult<DataModel?>(result);
    }
    protected override Task<Stream> ValueToStreamAsync(Stream s) {
        s.EncodeILInt(Value.Required().PayloadTagId);
        s.EncodeILInt(0); // deprecated field
        s.EncodeTagArray(Value.DataFields.Select(df => new ILTagDataField(df)));
        s.EncodeTagArray(Value.Indexes?.Select(index => new ILTagDataIndex(index)));
        s.EncodeString(Value.PayloadName);
        s.EncodeUShort(Value.Version);
        s.EncodeString(Value.Description.TrimToNull());
        return Task.FromResult(s);
    }
}