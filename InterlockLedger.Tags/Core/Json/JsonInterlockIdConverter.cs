/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public class JsonInterlockIdConverter : JsonConverter<InterlockId>
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(InterlockId) || typeToConvert.IsSubclassOf(typeof(InterlockId));

        public override InterlockId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.String ? InterlockId.Resolve(reader.GetString()) : throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, InterlockId value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.TextualRepresentation);
    }

}