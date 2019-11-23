/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace InterlockLedger.Tags
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Algorithm : ushort
    {
        RSA = 0,     // PKCS#1 RSASSA-PSS
        RSA15 = 1,   // RSASSA-PKCS1-v1_5
        DSA = 2,
        ElGamal = 3, // Signature
        EcDSA = 4,
        EdDSA = 5,
        Unknown = 0xFFFE,
        Invalid = 0xFFFF
    }
}