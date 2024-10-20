using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using WPFComponents.Model.Commands;
    using WPFComponents.Model.Interfaces;

    public class CommandActionConverter : JsonConverter<ICommandAction>
    {
        public override ICommandAction? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDocument = JsonDocument.ParseValue(ref reader);
            var rootElement = jsonDocument.RootElement;

            string? commandType = rootElement.GetProperty("CommandType").GetString();

            
            return commandType switch
            {
                "PressKeyCommand" => JsonSerializer.Deserialize<PressKeyCommand>(rootElement.GetRawText(), options),
                "OpenAppCommand" => JsonSerializer.Deserialize<OpenAppCommand>(rootElement.GetRawText(), options),
                _ => throw new NotSupportedException($"Тип команды '{commandType}' не поддерживается.")
            };
        }

        public override void Write(Utf8JsonWriter writer, ICommandAction value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case PressKeyCommand pressKeyCommand:
                    JsonSerializer.Serialize(writer, pressKeyCommand, options);
                    break;
                case OpenAppCommand openAppCommand:
                    JsonSerializer.Serialize(writer, openAppCommand, options);
                    break;
                default:
                    throw new NotSupportedException($"Тип команды '{value.GetType()}' не поддерживается для сериализации.");
            }
        }
    }

}
