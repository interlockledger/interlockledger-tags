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
    public enum KeyStrength
    {
        Normal = 0,         // RSA 2048
        Strong = 1,         // RSA 3072
        ExtraStrong = 2,    // RSA 4096
        MegaStrong = 3,     // RSA 5120
        SuperStrong = 4,    // RSA 6144
        HyperStrong = 5,    // RSA 7172
        UltraStrong = 6     // RSA 8192
    }
}
