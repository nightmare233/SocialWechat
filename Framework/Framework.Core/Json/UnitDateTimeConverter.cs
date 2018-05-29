using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Json
{
    public class UnitDateTimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                var ticks = (long)reader.Value;
                var date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                date.AddSeconds(ticks);

                return date;
            }

            return new JsonSerializer().Deserialize(reader, objectType);
        }

        public override bool CanWrite => true;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                long ticks = dateTime.Normalize().ToUnixTimeSeconds();
                writer.WriteValue(ticks);
                return;
            }

            if (value is DateTime?)
            {
                var dateTime = (DateTime?)value;
                long? ticks = dateTime.Normalize().ToUnixTimeSeconds();
                writer.WriteValue(ticks);
                return;
            }
        }
    }
}
