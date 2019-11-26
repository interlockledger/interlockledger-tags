/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Well we need to make it clear that we are casting to an integer number")]
    public enum CastType : byte
    {
        None = 0,
        DateTime = 1,
        Integer = 2,
        TimeSpan = 3
    }
}