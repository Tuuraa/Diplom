using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using FuzzySharp;
using WPFComponents.Model.Abstract;
using WPFComponents.Model.Commands;
using WPFComponents.Model.Utils;

namespace WPFComponents.Model
{
    /// <summary>
    /// Класс который отвечает за регистраницию комманд при запуске приложения.
    /// И обработку поступающих комманд в процессе работы приложения.
    /// </summary>
    public class VoiceCommandProcessor
    {
        private readonly Dictionary<string, Command> _commandsMap = new Dictionary<string, Command>();

        private List<string> _templates = new List<string>();

        private Dictionary<string,Scenario> scenarios = new Dictionary<string, Scenario>();

        public VoiceCommandProcessor()
        {
            
            _templates.Add("запусти сценарий");
            Scenario scenario = new Scenario();
            scenario.Phrases = new List<string> { "я дома" };
            scenario.Name = "Дом";
            scenario.Commands = new List<Command>();
            var ac = new NewsShowCommand();
            Command command = new Command();
            command.Action = ac;

            var ac1 = new OpenSiteCommand("https://habr.com/ru/flows/develop/news/");

            Command command1 = new Command();
            command1.Action = ac1;

            var ac2 = new OpenAppCommand(@"C:\Users\ivank\AppData\Roaming\Telegram Desktop\Telegram.exe");

            Command command2 = new Command();
            command2.Action = ac2;

            scenario.Commands.Add(command);
            scenario.Commands.Add(command1);
            scenario.Commands.Add(command2);

            scenarios.Add("я дома",scenario);
        }

        // Регистрация команды
        public void RegisterCommand(Command command)
        {
            foreach (var phrase in command.Phrases)
            {
                _commandsMap[phrase] = command;
            }

        }

        /// <summary>
        /// Перегрузка для коллекции комманд
        /// </summary>
        /// <param name="commands"></param>
        public void RegisterCommand(List<Command> commands)
        {
            foreach (var command in commands)
            {
                foreach (var phrase in command.Phrases)
                {
                    _commandsMap[phrase] = command;
                }
            }
        }

        /// <summary>
        /// Обработка распознанной голосовой фразы
        /// </summary>
        /// <param name="recognizedPhrase"></param>
        public void ProcessVoiceCommand(string recognizedPhrase)
        {
            if(recognizedPhrase.StartsWith("запусти сценарий", StringComparison.OrdinalIgnoreCase))
            {
                var bestScenario = FindBestFuzzyMatchScenario(recognizedPhrase.Substring("запусти сценарий".Length).Trim());
                if (bestScenario != null) {
                    ExecuteScenario(bestScenario);
                }
                return;
            }
            if (_commandsMap.ContainsKey(recognizedPhrase))
            {
                ExecuteCommand(_commandsMap[recognizedPhrase], recognizedPhrase);
            }
            else
            {
                // Если точное совпадение не найдено, используем нечеткое сравнение
                var bestMatch = FindBestFuzzyMatch(recognizedPhrase);
                if (bestMatch != null)
                {
                    ExecuteCommand(bestMatch, recognizedPhrase);
                }
                else
                {
                    MessageBox.Show($"Неизвестная команда: {recognizedPhrase}");
                }
            }
        }

        /// <summary>
        /// Метод для выполнения команды
        /// </summary>
        /// <param name="command"></param>
        /// <param name="recognizedPhrase"></param>
        private void ExecuteCommand(Command command, string recognizedPhrase)
        {
            if (command.Action.CanExecute())
            {
                command.Action.Execute();
            }
            else
            {
                MessageBox.Show($"Команда '{recognizedPhrase}' не может быть выполнена.");
            }
        }

        /// <summary>
        /// Метод для поиска команды с использованием нечеткого сравнения
        /// </summary>
        /// <param name="recognizedPhrase"></param>
        /// <returns></returns>
        private Command FindBestFuzzyMatch(string recognizedPhrase)
        {
            Command bestMatchCommand = null;
            int highestScore = 0;

            foreach (var entry in _commandsMap)
            {
                // Оценка похожести фраз с помощью библиотеки FuzzySharp
                var score = Fuzz.Ratio(entry.Key, recognizedPhrase);

                // Устанавливаем порог, например 80
                if (score > 65 && score > highestScore)
                {
                    highestScore = score;
                    bestMatchCommand = entry.Value;
                }
            }

            return bestMatchCommand;
        }

        private Scenario FindBestFuzzyMatchScenario(string recognizedPhrase)
        {
            Scenario bestMatchCommand = null;
            int highestScore = 0;

            foreach (var entry in scenarios)
            {
                // Оценка похожести фраз с помощью библиотеки FuzzySharp
                var score = Fuzz.Ratio(entry.Key, recognizedPhrase);

                // Устанавливаем порог, например 80
                if (score > 65 && score > highestScore)
                {
                    highestScore = score;
                    bestMatchCommand = entry.Value;
                }
            }

            return bestMatchCommand;
        }

        private void ExecuteScenario(Scenario scenario)
        {
            foreach(var command in scenario.Commands)
            {
                ExecuteCommand(command,scenario.Name);
            }
        }
    }
}
