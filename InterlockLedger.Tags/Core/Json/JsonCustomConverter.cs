/******************************************************************************************************************************
 *
 *      Copyright (c) 2017-2019 InterlockLedger Network
 *
 ******************************************************************************************************************************/

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InterlockLedger.Tags
{
    public class JsonCustomConverter<T> : JsonConverter<T> where T : IJsonCustom<T>, new()
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(T) || typeToConvert.IsSubclassOf(typeof(T));

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => reader.TokenType == JsonTokenType.String ? new T().ResolveFrom(reader.GetString()) : throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.TextualRepresentation);
    }
}