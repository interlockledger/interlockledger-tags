/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    [SuppressMessage("Design", "CA1027:Mark enums with FlagsAttribute", Justification = "This is not to be bit-ored")]
    public enum KeyPurpose : ulong
    {
        InvalidKey = 0,
        KeyManagement = 1,
        Action = 2,
        ClaimSigner = 3,
        Encryption = 4,
        Protocol = 5,
        ChainOperation = 7,
        ForceInterlock = 8
        // EmergencyClosing = 6, Now use specific action id
    }

    public static class KeyPurposeExtensions
    {
        public static string ToStringAsList(this IEnumerable<KeyPurpose> purposes) => purposes.OrderBy(p => p).WithCommas();
    }
}