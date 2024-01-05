// ******************************************************************************************************************************
//  
// Copyright (c) 2018-2024 InterlockLedger Network
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

using System.Security.Cryptography;

namespace InterlockLedger.Tags;
public static class KeyECDsaExtensions
{
    //  enum KeyStrength
    //  Normal = 0,         // ECDsa 192
    //  Strong = 1,         // ECDsa 256
    //  ExtraStrong = 2,    // ECDsa 320
    //  MegaStrong = 3,     // ECDsa 384
    //  SuperStrong = 4,    // ECDsa 448
    //  HyperStrong = 5,    // ECDsa 512
    //  UltraStrong = 6     // ECDsa 576

    public static KeyStrength KeyStrengthGuess(this ECDsa key) => Guess(key.Required().KeySize);

    public static int ECDsaKeySize(this KeyStrength strength) => 192 + (64 * (int)strength);

    private static KeyStrength Guess(int size)
        => size <= 192
            ? KeyStrength.Normal
            : size <= 256
                ? KeyStrength.Strong
                : size <= 320
                    ? KeyStrength.ExtraStrong
                    : size <= 384
                        ? KeyStrength.MegaStrong
                        : size <= 448
                            ? KeyStrength.SuperStrong
                            : size <= 512
                                ? KeyStrength.HyperStrong
                                : KeyStrength.UltraStrong;
}