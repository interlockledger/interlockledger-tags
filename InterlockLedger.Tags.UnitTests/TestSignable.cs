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
internal class TestSignable(ulong someILInt) : Signable<TestSignable>(FieldTagId, CurrentVersion)
{
    public const ushort CurrentVersion = 1;
    public const ulong FieldTagId = 1_000_000;

    public TestSignable() : this(0ul) {
    }
    public ulong SomeILInt { get; private set; } = someILInt;
    protected override IEnumerable<DataField> RemainingStateFields { get; } = new DataField {
        Name = nameof(SomeILInt),
        TagId = ILTagId.ILInt
    }.AsSingle();

    protected override string TypeDescription => "Signable for unit testing";

    protected override Task DecodeRemainingStateFromAsync(Stream s) {
        SomeILInt = s.DecodeILInt();
        return Task.CompletedTask;
    }
    protected override Task EncodeRemainingStateToAsync(Stream s) {
        s.EncodeILInt(SomeILInt);
        return Task.CompletedTask;
    }
}