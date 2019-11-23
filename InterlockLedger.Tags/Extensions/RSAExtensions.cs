/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
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
