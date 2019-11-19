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
    public enum CipherAlgorithm : ushort
    {
        AES256 = 0,
        None = 0xFFFF,
    }
}