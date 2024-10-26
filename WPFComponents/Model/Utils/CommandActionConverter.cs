using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model.Utils
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Windows;
    using WPFComponents.Model.Commands;
    using WPFComponents.Model.Interfaces;

    public class CommandActionConverter : JsonConverter<ICommandAction>
    {
        public override ICommandAction? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonObject = JsonDocument.ParseValue(ref reader).RootElement;
            string? commandType = jsonObject.GetProperty("commandType").GetString();

            switch (commandType)
            {
                case "OpenAppCommand":
                    return JsonSerializer.Deserialize<OpenAppCommand>(jsonObject.GetRawText(), options);
                case "NewsShowCommand":
                    return JsonSerializer.Deserialize<NewsShowCommand>(jsonObject.GetRawText(), options);
                default:
                    throw new Exception("Неизвестный тип команды");
            }
            throw new NotSupportedException($"Command type {commandType} is not supported.");
        }

        public override void Write(Utf8JsonWriter writer, ICommandAction value, JsonSerializerOptions options)
        {
            string commandType = value.GetType().Name;
            writer.WriteStartObject();
            writer.WriteString("commandType", commandType);
            switch (value)
            {
                case PressKeyCommand pressKeyCommand:
                    writer.WriteString("key", pressKeyCommand.Key);
                    //JsonSerializer.Serialize(writer, pressKeyCommand, options);
                    break;
                case NewsShowCommand newsShowcommand:
                    //JsonSerializer.Serialize(writer, newsShowcommand, options);
                    break;
                default:
                    throw new NotSupportedException($"Тип команды '{value.GetType()}' не поддерживается для сериализации.");
            }
            writer.WriteEndObject();
        }
    }

}
