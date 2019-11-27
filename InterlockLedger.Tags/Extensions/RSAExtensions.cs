/******************************************************************************************************************************
 
Copyright (c) 2018-2019 InterlockLedger Network
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of the copyright holder nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

******************************************************************************************************************************/

using System;
using System.Security.Cryptography;

namespace InterlockLedger.Tags
{
#pragma warning disable S101 // Types should be named in PascalCase

    public static class RSAExtensions
    {
        //  enum KeyStrength
        //  Normal = 0,         // RSA 2048
        //  Strong = 1,         // RSA 3072
        //  ExtraStrong = 2,    // RSA 4096
        //  MegaStrong = 3,     // RSA 5120
        //  SuperStrong = 4,    // RSA 6144
        //  HyperStrong = 5,    // RSA 7172
        //  UltraStrong = 6     // RSA 8192

        public static KeyStrength KeyStrengthGuess(this RSA key) {
            if (key is null)
                throw new ArgumentNullException(nameof(key));
            var size = key.KeySize;
            if (size <= 2048) return KeyStrength.Normal;
            if (size <= 3072) return KeyStrength.Strong;
            if (size <= 4096) return KeyStrength.ExtraStrong;
            if (size <= 5120) return KeyStrength.MegaStrong;
            if (size <= 6144) return KeyStrength.SuperStrong;
            if (size <= 7172) return KeyStrength.HyperStrong;
            return KeyStrength.UltraStrong;
        }

        public static int RSAKeySize(this KeyStrength strength) => 2048 + (1024 * (int)strength);
    }
}
