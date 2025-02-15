using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using WindowsDesktop;
using WPFComponents.DB;
using WPFComponents.Model.Commands;
using WPFComponents.Services;

namespace WPFComponents.Model
{
    public class VoiceCommandProcessor
    {
        private readonly CommandMatcher _commandMatcher;
        //private readonly ScenarioMatcher _scenarioMatcher;
        private readonly LLMActionService _llmService;
        private readonly LoggerService _logger;

        public VoiceCommandProcessor(
            LoggerService logger,
            LLMActionService llmService,
            ApplicationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _llmService = llmService;
            var commands = context.Commands.ToList();
            var commandsMap = ConvertCommandsToMap(commands);
            var scenarios = InitializeScenarios();

            _commandMatcher = new CommandMatcher(commandsMap);
            //_scenarioMatcher = new ScenarioMatcher(scenarios);
        }

        private Dictionary<string, Command> ConvertCommandsToMap(List<Command> commands)
        {
            var map = new Dictionary<string, Command>();
            foreach (var command in commands)
            {
                foreach (var phrase in command.Phrases)
                {
                    map[phrase] = command;
                }
            }
            return map;
        }

        private Dictionary<string, Scenario> InitializeScenarios()
        {
            var scenarios = new Dictionary<string, Scenario>();

            var homeScenario = new Scenario
            {
                Name = "Дом",
                Phrases = new List<string> { "я дома" },
                Commands = new List<Command>
                {
                    new Command { Action = new NewsShowCommand() },
                    new Command { Action = new OpenSiteCommand("https://habr.com/ru/flows/develop/news/") },
                    new Command { Action = new OpenAppCommand(@"C:\Users\ivank\AppData\Roaming\Telegram Desktop\Telegram.exe") }
                }
            };

            scenarios[homeScenario.Name] = homeScenario;
            return scenarios;
        }

        public async Task ProcessVoiceCommand(string recognizedPhrase)
        {
            try
            {
                // Обработка локальных команд
                var commandResult = _commandMatcher.Match(recognizedPhrase);
                if (commandResult.Confidence > 0.4)
                {
                    await ExecuteCommand(commandResult.Command, recognizedPhrase);
                    return;
                }

                // Обработка сценариев
                //var scenarioResult = _scenarioMatcher.Match(recognizedPhrase);
                //if (scenarioResult.Confidence > 0.4)
                //{
                //    ExecuteScenario(scenarioResult.Scenario);
                //    return;
                //}

                // Асинхронный вызов LLM без блокировки
                _ = ProcessWithLLMAsync(recognizedPhrase);

                // Уведомление пользователя
                NotifyUser("Команда не распознана");
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Ошибка обработки команды: {ex.Message}");
            }
        }

        private async Task ProcessWithLLMAsync(string phrase)
        {
            try
            {
                var code = await _llmService.GenerateCodeAsync(phrase);
                if (!string.IsNullOrWhiteSpace(code))
                {
                    await _llmService.ExecuteGeneratedCodeAsync(code);
                    //_logger.LogLLMAction(phrase, code);
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Ошибка LLM: {ex.Message}");
            }
        }

        private async Task ExecuteCommand(Command command, string recognizedPhrase)
        {
            if (command.Action.CanExecute())
            {
                //_logger.LogCommand(recognizedPhrase);
                await Task.Run(() => command.Action.Execute());
            }
            else
            {
                NotifyUser($"Команда не может быть выполнена: {recognizedPhrase}");
            }
        }

        private void ExecuteScenario(Scenario scenario)
        {
            try
            {
                VirtualDesktop.Create().Switch();
                //_logger.LogScenario(scenario.Name);

                foreach (var command in scenario.Commands)
                {
                    if (command.Action.CanExecute())
                        Task.Run(() => command.Action.Execute());
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Ошибка выполнения сценария: {ex.Message}");
            }
        }

        private void NotifyUser(string message)
        {
            // Реализация уведомлений
        }
    }
}
    

