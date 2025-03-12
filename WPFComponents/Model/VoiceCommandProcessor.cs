using H.NotifyIcon;
using SkyUtils;
using System.Windows.Controls.Primitives;
using WindowsDesktop;
using WPFComponents.Model.Commands;
using WPFComponents.Services;
using WPFComponents.Utils;
using WPFComponents.View;
using ApplicationContext = WPFComponents.DB.ApplicationContext;
using Scenario = WPFComponents.DB.Scenario;

namespace WPFComponents.Model
{
    public class VoiceCommandProcessor
    {
        private readonly CommandMatcher _commandMatcher;
        private readonly ScenarioMatcher _scenarioMatcher;
        private readonly LLMActionService _llmService;
        private readonly LoggerService _logger;
        public TaskbarIcon? TrayIcon;

        public VoiceCommandProcessor(
            LoggerService logger,
            LLMActionService llmService,
            ApplicationContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _llmService = llmService;
            context.Database.EnsureCreated();
            var commands = context.Commands.ToList();
            var commandsMap = ConvertCommandsToMap(commands);
            var scenarios = context.Scenarios.ToList();
            var scenariosMap = ConvertScenariosToMap(scenarios);

            var test = _logger.GetLogs();
            var stop = 5;

            _commandMatcher = new CommandMatcher(commandsMap);
            _scenarioMatcher = new ScenarioMatcher(scenariosMap);
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
        private Dictionary<string, Scenario> ConvertScenariosToMap(List<Scenario> scenarios)
        {
            var map = new Dictionary<string, Scenario>();
            foreach (var scenario in scenarios)
            {
                foreach (var phrase in scenario.Phrases)
                {
                    map[phrase] = scenario;
                }
            }
            return map;
        }

        public async Task ProcessVoiceCommand(string recognizedPhrase)
        {
            try
            {

                var scenarioResult = _scenarioMatcher.Match(recognizedPhrase);
                if (scenarioResult.Confidence > 0.4)
                {
                    _logger.LogCommand("Вызов сценария" + scenarioResult.Scenario.Name);
                    await ExecuteScenario(scenarioResult.Scenario);
                    return;
                }

                // Обработка локальных команд
                var commandResult = _commandMatcher.Match(recognizedPhrase);
                if (commandResult.Confidence > 0.4)
                {
                    _logger.LogCommand(commandResult.Command.Name);
                    await ExecuteCommand(commandResult.Command, recognizedPhrase);
                    return;
                }

                

                //_ = ProcessWithLLMAsync(recognizedPhrase);

                // Уведомление пользователя
                //NotifyUser("Команда не распознана");
            }
            catch (Exception ex)
            {
                _logger.LogCommand($"Ошибка обработки команды: {ex.Message}");
            }
        }

        private async Task ProcessWithLLMAsync(string phrase)
        {
            try
            {
                var code = await _llmService.GenerateCodeAsync(phrase);
                await NotifyUserAsync(code);
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

        private async Task ExecuteScenario(Scenario scenario)
        {
            try
            {
                //VirtualDesktop.Create().Switch();
                //_logger.LogScenario(scenario.Name);

                foreach (var command in scenario.Commands)
                {
                    if (command.Action.CanExecute())
                        await Task.Run(() => command.Action.Execute());
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //_logger.LogError($"Ошибка выполнения сценария: {ex.Message}");
            }
        }

        private void NotifyUser(string message)
        {
            AgreeBalloon balloon = new AgreeBalloon(message);
            //balloon.AgreeClicked += Balloon_AgreeClicked;
            TrayIcon.ShowNotification("test",message);
        }
        private async Task NotifyUserAsync(string message)
        {
            var tcs = new TaskCompletionSource<bool>();

            // Создание и настройка FancyBalloon
            FancyBalloon balloon = new FancyBalloon(message);
            balloon.AgreeClicked += (sender, e) =>
            {
                // Завершаем задачу, когда пользователь нажал кнопку
                tcs.SetResult(true);
                TrayIcon.CloseBalloon();
            };

            // Показать баллон
            TrayIcon.ShowCustomBalloon(balloon, PopupAnimation.Fade, 50000);

            // Ожидаем, пока пользователь согласится
            await tcs.Task;
        }
    }
}
    

