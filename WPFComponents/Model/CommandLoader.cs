using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json;
using WPFComponents.Model.Commands;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model
{
    public class CommandLoader
    {
        //private readonly DataBaseContext _dbContext;
        //private readonly VoiceCommandProcessor _voiceCommandProcessor;

        //public CommandLoader(VoiceCommandProcessor voiceCommandProcessor)
        //{
        //    _dbContext = new DataBaseContext();
        //    _voiceCommandProcessor = voiceCommandProcessor;
        //}

        //public void LoadAndRegisterCommands()
        //{
        //    var commands = _dbContext.Commands
        //        .ToList();

        //    foreach (var command in commands)
        //    {
        //        // Создаём действие на основе типа и параметров команды
        //        //ICommandAction action = CreateActionFromTypeAndParameters(command.Type, command.Parameters);
        //        var voiceCommand = new Command
        //        {
        //            Id = command.Id,
        //            Name = command.Name,
        //            Type = command.Type, // Обновлено: сохраняем тип команды
        //            Parameters = command.Parameters, // Обновлено: сохраняем параметры
        //            Phrases = command.Phrases,
        //        };
        //        // Регистрируем каждую фразу, связанную с командой
        //        _voiceCommandProcessor.RegisterCommand(voiceCommand);
        //    }
        //}

        //private ICommandAction CreateActionFromTypeAndParameters(string type, string parametersJson)
        //{
        //    // Парсим JSON и создаём соответствующую команду
        //    //switch (type)
        //    //{
        //    //    case "OpenAppCommand":
        //    //        var appParams = JsonSerializer.Deserialize<OpenAppParameters>(parametersJson);
        //    //        return new OpenAppCommand(appParams.PathToExe);

        //    //    case "PressKeyCommand":
        //    //        var keyParams = JsonSerializer.Deserialize<PressKeyParameters>(parametersJson);
        //    //        return new PressKeyCommand(keyParams.Key);

        //    //    // Добавьте дополнительные команды по мере необходимости
        //    //    default:
        //    //        throw new InvalidOperationException($"Unknown command type: {type}");
        //    //}
        //}
    }
}
