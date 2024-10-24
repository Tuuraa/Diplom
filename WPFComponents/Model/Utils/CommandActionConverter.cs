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

            if (commandType == "OpenAppCommand")
            {
                return JsonSerializer.Deserialize<OpenAppCommand>(jsonObject.GetRawText(), options);
            }
            switch (commandType)
            {
                case "OpenAppCommand":
                    return JsonSerializer.Deserialize<OpenAppCommand>(jsonObject.GetRawText(), options);
                case "PressKeyCommand":
                    return JsonSerializer.Deserialize<PressKeyCommand>(jsonObject.GetRawText(), options);
                default:
                    throw new Exception("Неизвестный тип команды");
            }
            throw new NotSupportedException($"Command type {commandType} is not supported.");
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
