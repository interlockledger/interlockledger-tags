/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#pragma warning disable CA2227 // Collection properties should be read only

namespace InterlockLedger.Tags
{
    [JsonConverter(typeof(StringEnumConverter))]
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Well we need to make it clear that we are casting to an integer number")]
    public enum CastType : byte
    {
        None = 0,
        DateTime = 1,
        Integer = 2,
        TimeSpan = 3
    }
}