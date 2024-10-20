using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using FuzzySharp;

namespace WPFComponents.Model
{
    public class VoiceCommandProcessor
    {
        private readonly Dictionary<string, Command> _commandsMap = new Dictionary<string, Command>();
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Media", "commands.json");

        public VoiceCommandProcessor()
        {
            DeserializeCommand();
        }

        // Регистрация команды
        public void RegisterCommand(Command command)
        {
            _commandsMap[command.Phrase] = command;

            /*var succes = CommandSerializer();

            if (!succes)
            {
                //Обработка ошибки сериализации
            }*/
        }

        // Обработка распознанной голосовой фразы
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

        // Метод для выполнения команды
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

        // Метод для поиска команды с использованием нечеткого сравнения
        private Command FindBestFuzzyMatch(string recognizedPhrase)
        {
            Command bestMatchCommand = null;
            int highestScore = 0;

            foreach (var entry in _commandsMap)
            {
                // Оценка похожести фраз с помощью библиотеки FuzzySharp
                var score = Fuzz.Ratio(entry.Key, recognizedPhrase);

                // Устанавливаем порог, например 80
                if (score > 80 && score > highestScore)
                {
                    highestScore = score;
                    bestMatchCommand = entry.Value;
                }
            }

            return bestMatchCommand;
        }

        //Сериализация новых команд
        public bool CommandSerializer()
        {
            try
            {
                List<Command> commands = _commandsMap.Values.ToList();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new CommandActionConverter() }
                };

                string json = JsonSerializer.Serialize(commands, options);
                File.WriteAllText(_filePath, json);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex is " + ex.Message);
                return false;
            }
        }

        private List<CommandBase> DeserializeCommand()
        {
            try
            {
                string json = File.ReadAllText(_filePath);

                var options = new JsonSerializerOptions
                {
                    Converters = { new CommandActionConverter() },
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                List<CommandBase> commands = JsonSerializer.Deserialize<List<CommandBase>>(json, options);

                if (commands != null && commands.Count > 0)
                {
                    foreach (var command in commands)
                    {
                        MessageBox.Show($"type: {command.GetType()}");
                    }
                }
                else
                {
                    MessageBox.Show("Команды не найдены или пусты.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}");
            }

            return null;
        }



    }
}
