using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infraestructure.Serialization
{
    /// <summary>
    /// Serializa DateTime en reloj local, sin Z ni offset, para que OPENJSON no convierta a UTC.
    /// </summary>
    internal sealed class LocalClockDateTimeJsonConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-ddTHH:mm:ss.fffffff";

        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var text = reader.GetString();
            if (string.IsNullOrWhiteSpace(text))
            {
                return default;
            }

            return DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var local = value.Kind == DateTimeKind.Utc ? value.ToLocalTime() : value;
            writer.WriteStringValue(
                DateTime.SpecifyKind(local, DateTimeKind.Unspecified)
                    .ToString(Format, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Serializa DateTime? con el mismo reloj local sin zona.
    /// </summary>
    internal sealed class NullableLocalClockDateTimeJsonConverter : JsonConverter<DateTime?>
    {
        private static readonly LocalClockDateTimeJsonConverter Inner = new();

        /// <inheritdoc />
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            return Inner.Read(ref reader, typeof(DateTime), options);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            Inner.Write(writer, value.Value, options);
        }
    }
}
