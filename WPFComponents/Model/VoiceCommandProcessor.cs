using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using FuzzySharp;
using WPFComponents.Model.Abstract;
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

        public VoiceCommandProcessor()
        {
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
            // Попытка точного соответствия
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
    }
}
