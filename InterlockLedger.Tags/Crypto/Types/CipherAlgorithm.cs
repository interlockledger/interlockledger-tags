/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CipherAlgorithm : ushort
    {
        AES256 = 0,
        None = 0xFFFF,
    }
}