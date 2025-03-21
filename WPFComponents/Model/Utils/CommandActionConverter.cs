using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model.Utils
{
    using SkyUtils;
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
                    //return JsonSerializer.Deserialize<OpenAppCommand>(jsonObject.GetRawText(), options);
                    string path = jsonObject.GetProperty("PathToExe").GetString();
                    return new OpenAppCommand(path);
                case "NewsShowCommand":
                    return JsonSerializer.Deserialize<NewsShowCommand>(jsonObject.GetRawText(), options);
                case "MoveMouseCommand":
                    //return JsonSerializer.Deserialize<MoveMouseCommand>(jsonObject.GetRawText(), options);
                    int x = jsonObject.GetProperty("X").GetInt32();
                    int y = jsonObject.GetProperty("Y").GetInt32();
                    bool click = jsonObject.GetProperty("Click").GetBoolean();
                    return new MoveMouseCommand(x, y, click, commandType);
                case "PressKeyCommand":
                    string key = jsonObject.GetProperty("key").GetString() ?? string.Empty;
                    return new PressKeyCommand(key, commandType);
                case "PrintWordCommand":
                    string word = jsonObject.GetProperty("Word").GetString() ?? string.Empty;
                    return new PrintWordCommand(word);
                case "HideWindowCommand":
                    return new HideWindowCommand();
                case "DrawCircleCommand":
                    int radius = jsonObject.GetProperty("Radius").GetInt32();
                    return new DrawCircleCommand(radius, commandType);
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
                case MoveMouseCommand moveMouseCommand:
                    writer.WriteNumber("X", moveMouseCommand.X);
                    writer.WriteNumber("Y", moveMouseCommand.Y);
                    writer.WriteBoolean("Click", moveMouseCommand.Click);
                    break;
                case PrintWordCommand printWordCommand:
                    writer.WriteString("Word", printWordCommand.Word);
                    break;
                case HideWindowCommand hideWindowCommand:
                    break;
                case OpenAppCommand openAppCommand:
                    writer.WriteString("PathToExe", openAppCommand.PathToExe);
                    break;
                case DrawCircleCommand drawCircleCommand:
                    writer.WriteNumber("Radius", drawCircleCommand.Radius);
                    break;
                default:
                    throw new NotSupportedException($"Тип команды '{value.GetType()}' не поддерживается для сериализации.");
            }
            writer.WriteEndObject();
        }
    }

}
