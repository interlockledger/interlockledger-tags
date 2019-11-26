/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum HashAlgorithm : ushort
    {
        SHA1 = 0,
        SHA256 = 1,
        SHA512 = 2,
        SHA3_256 = 3,
        SHA3_512 = 4,

        Copy = 0xFFFF
    }
}