using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPFComponents.Services;
using WPFComponents.DB;
using SkyUtils;
using Scenario = WPFComponents.DB.Scenario;

namespace WPFComponents.Model
{
    public class MatchResult
    {
        public Command Command { get; }
        public Scenario Scenario { get; }
        public float Confidence { get; }
        public bool IsLLMFallback { get; }

        public MatchResult(Command command, float confidence, bool isLlmFallback = false)
        {
            Command = command;
            Confidence = confidence;
            IsLLMFallback = isLlmFallback;
        }
        public MatchResult(Scenario scenario, float confidence, bool isLlmFallback = false)
        {
            Scenario = scenario;
            Confidence = confidence;
            IsLLMFallback = isLlmFallback;
        }
    }

    // Класс для нормализации текста
    public static class TextNormalizer
    {
        private static readonly Regex _cleanRegex = new Regex("[^а-яa-z0-9 ]", RegexOptions.Compiled);
        private static readonly HashSet<string> _stopWords = new HashSet<string>
        {
        "пожалуйста", "найди", "сделай", "запусти", "открой", "мне", "нужно"
        };

        public static string Normalize(string input)
        {
            // Приведение к нижнему регистру и удаление спецсимволов
            var cleaned = _cleanRegex.Replace(input.ToLowerInvariant(), " ");

            // Удаление стоп-слов
            var words = cleaned.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => !_stopWords.Contains(w));

            return string.Join(" ", words);
        }
    }

    // Базовый класс для сопоставления команд
    public interface ICommandMatcher
    {
        MatchResult Match(string phrase);
    }

    public interface IScenarioMatcher
    {
        MatchResult Match(string phrase);
    }

    // Реализация CommandMatcher
    public class CommandMatcher : ICommandMatcher
    {
        private readonly Dictionary<string, Command> _exactMatches = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
        private readonly LevenshteinMatcher _levenshteinMatcher;
        private readonly TfidfMatcher _tfidfMatcher;

        // Убрали зависимость от ILlmService из конструктора
        public CommandMatcher(Dictionary<string, Command> commands)
        {
            //_exactMatches = commands;
            foreach (var kvp in commands)
            {
                _exactMatches[kvp.Key] = kvp.Value;
            }
            _levenshteinMatcher = new LevenshteinMatcher(commands);
            _tfidfMatcher = new TfidfMatcher(commands);
        }

        public MatchResult Match(string phrase)
        {
            var normalized = TextNormalizer.Normalize(phrase);

            // Этап 1: Точное совпадение
            if (_exactMatches.TryGetValue(normalized, out var exactCommand))
                return new MatchResult(exactCommand, 1.0f);

            // Этап 2: Левенштейн для коротких фраз
            if (normalized.Length < 15)
            {
                var levResult = _levenshteinMatcher.Match(normalized);
                if (levResult.Confidence > 0.8f)
                    return levResult;
            }

            // Этап 3: TF-IDF
            var tfidfResult = _tfidfMatcher.Match(normalized);
            if (tfidfResult.Confidence > 0.4f)
                return tfidfResult;

            // Этап 4: Не найдено (офлайн режим)
            return new MatchResult(new Command(), 0f);
        }
        public MatchResult MatchScenario(string phrase)
        {
            var normalized = TextNormalizer.Normalize(phrase);

            // Этап 1: Точное совпадение
            if (_exactMatches.TryGetValue(normalized, out var exactCommand))
                return new MatchResult(exactCommand, 1.0f);

            // Этап 2: Левенштейн для коротких фраз
            if (normalized.Length < 15)
            {
                var levResult = _levenshteinMatcher.Match(normalized);
                if (levResult.Confidence > 0.8f)
                    return levResult;
            }

            // Этап 3: TF-IDF
            var tfidfResult = _tfidfMatcher.Match(normalized);
            if (tfidfResult.Confidence > 0.4f)
                return tfidfResult;

            // Этап 4: Не найдено (офлайн режим)
            return new MatchResult(new Scenario(), 0f);
        }
    }

    public class LevenshteinMatcher : ICommandMatcher
    {
        private readonly Dictionary<string, Command>? _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Scenario>? _scenarios;

        public LevenshteinMatcher(Dictionary<string, Command> commands)
        {
            foreach (var kvp in commands)
            {
                _commands[kvp.Key] = kvp.Value;
            }
            //_commands = commands;
        }
        public LevenshteinMatcher(Dictionary<string, Scenario> scenarios)
        {
            _scenarios = scenarios;
        }

        public MatchResult Match(string phrase)
        {
            Command bestMatch = null;
            float bestScore = 0;

            foreach (var key in _commands.Keys)
            {
                var score = FastLevenshtein.GetSimilarity(phrase, key);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = _commands[key];
                }
            }

            return new MatchResult(bestMatch, bestScore);
        }
        public MatchResult MatchScenario(string phrase)
        {
            Scenario bestMatch = null;
            float bestScore = 0;

            foreach (var key in _scenarios.Keys)
            {
                var score = FastLevenshtein.GetSimilarity(phrase, key);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = _scenarios[key];
                }
            }

            return new MatchResult(bestMatch, bestScore);
        }
    }

    // Реализация матчера на TF-IDF
    public class TfidfMatcher : ICommandMatcher
    {
        private readonly Dictionary<string, Command> _commands;
        private readonly Dictionary<string, Scenario> _scenarios;
        private readonly TfIdfProcessor _tfidf;

        public TfidfMatcher(Dictionary<string, Command> commands)
        {
            _commands = commands;
            _tfidf = new TfIdfProcessor();

            foreach (var key in commands.Keys)
                _tfidf.AddDocument(key);
        }
        public TfidfMatcher(Dictionary<string, Scenario> scenarios)
        {
            _scenarios = scenarios;
            _tfidf = new TfIdfProcessor();

            foreach (var key in scenarios.Keys)
                _tfidf.AddDocument(key);
        }

        public MatchResult Match(string phrase)
        {
            var bestScore = 0f;
            Command bestCommand = null;

            foreach (var (key, command) in _commands)
            {
                var score = _tfidf.CalculateSimilarity(phrase, key);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCommand = command;
                }
            }

            return new MatchResult(bestCommand, bestScore);
        }
        public MatchResult MatchScenario(string phrase)
        {
            var bestScore = 0f;
            Scenario bestCommand = null;

            foreach (var (key, command) in _scenarios)
            {
                var score = _tfidf.CalculateSimilarity(phrase, key);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestCommand = command;
                }
            }

            return new MatchResult(bestCommand, bestScore);
        }
    }
}
