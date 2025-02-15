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
        private readonly Dictionary<string, Command> _commandsMap = new();
        private readonly Dictionary<string, Scenario> _scenarios = new();
        private readonly Dictionary<string, double[]> _tfidfVectors = new();
        private readonly HashSet<string> _vocabulary = new();
        private readonly LoggerService _logger;


        public VoiceCommandProcessor(LoggerService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitializeScenarios();
            BuildVocabulary();
            ComputeTfIdfVectors();
        }
        public VoiceCommandProcessor(List<Command> commands, LoggerService logger)
            : this(logger) 
        {
            RegisterCommand(commands);
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
            BuildVocabulary();
            ComputeTfIdfVectors();
        }

        //TODO: Удлаить и сделать через БД
        private void InitializeScenarios()
        {
            // Пример сценария
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

            _scenarios["я дома"] = homeScenario;
        }

        private void BuildVocabulary()
        {
            foreach (var phrase in _commandsMap.Keys.Concat(_scenarios.Keys))
            {
                foreach (var word in Tokenize(phrase))
                {
                    _vocabulary.Add(word);
                }
            }
        }

        private void ComputeTfIdfVectors()
        {
            var documents = _commandsMap.Keys.Concat(_scenarios.Keys).ToList();
            var documentCount = documents.Count;
            var wordDocumentFrequency = new Dictionary<string, int>();

            // Считаем частоту документов для каждого слова
            foreach (var word in _vocabulary)
            {
                wordDocumentFrequency[word] = documents.Count(doc => Tokenize(doc).Contains(word));
            }

            // Рассчитываем TF-IDF для каждой команды/сценария
            foreach (var document in documents)
            {
                var termFrequency = Tokenize(document)
                    .GroupBy(word => word)
                    .ToDictionary(group => group.Key, group => group.Count());

                double[] tfidfVector = _vocabulary
                    .Select(word =>
                    {
                        var tf = termFrequency.ContainsKey(word) ? termFrequency[word] : 0;
                        var idf = Math.Log((double)documentCount / (1 + wordDocumentFrequency[word]));
                        return tf * idf;
                    })
                    .ToArray();

                _tfidfVectors[document] = tfidfVector;
            }
        }

        private static IEnumerable<string> Tokenize(string text)
        {
            return text.ToLowerInvariant().Split(new[] { ' ', ',', '.', '!' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private double ComputeCosineSimilarity(double[] vectorA, double[] vectorB)
        {
            var dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
            var magnitudeA = Math.Sqrt(vectorA.Sum(a => a * a));
            var magnitudeB = Math.Sqrt(vectorB.Sum(b => b * b));
            return dotProduct / (magnitudeA * magnitudeB);
        }

        public async void ProcessVoiceCommand(string recognizedPhrase)
        {
            var bestMatchCommand = FindBestMatch(_commandsMap, recognizedPhrase);
            if (bestMatchCommand != null)
            {
                ExecuteCommand(bestMatchCommand, recognizedPhrase);
                return;
            }

            var bestMatchScenario = FindBestMatch(_scenarios, recognizedPhrase);
            if (bestMatchScenario != null)
            {
                ExecuteScenario(bestMatchScenario);
                return;
            }
        }

        private T FindBestMatch<T>(Dictionary<string, T> items, string input)
        {
            var inputVector = ComputeTfIdfVector(input);
            double highestSimilarity = 0;
            T bestMatch = default;

            foreach (var (key, value) in items)
            {
                var similarity = ComputeCosineSimilarity(inputVector, _tfidfVectors[key]);
                if (similarity > highestSimilarity)
                {
                    highestSimilarity = similarity;
                    bestMatch = value;
                }
            }

            return highestSimilarity > 0.1 ? bestMatch : default; // Порог для релевантности
        }

        private double[] ComputeTfIdfVector(string input)
        {
            var termFrequency = Tokenize(input)
                .GroupBy(word => word)
                .ToDictionary(group => group.Key, group => group.Count());

            return _vocabulary
                .Select(word =>
                {
                    var tf = termFrequency.ContainsKey(word) ? termFrequency[word] : 0;
                    var idf = Math.Log((double)_tfidfVectors.Count / (1 + _tfidfVectors.Values.Count(v => v[_vocabulary.ToList().IndexOf(word)] > 0)));
                    return tf * idf;
                })
                .ToArray();
        }

        private void ExecuteCommand(Command command, string recognizedPhrase)
        {
            if (command.Action.CanExecute())
            {
                _logger.LogCommand(recognizedPhrase);
                command.Action.Execute();
            }
            else
            {
                //NotifyUserFail($"Команда \"{recognizedPhrase}\" не может быть выполнена.");
            }
        }

        private void ExecuteScenario(Scenario scenario)
        {
            VirtualDesktop.Create().Switch();
            _logger.LogCommand(scenario.Name);
            foreach (var command in scenario.Commands)
            {
                ExecuteCommand(command, scenario.Name);
            }
        }
    }
}
    

