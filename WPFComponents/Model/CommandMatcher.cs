using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WPFComponents.Services;
using WPFComponents.DB;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Math.Distances;
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

    public interface ICommandMatcher
    {
        MatchResult Match(string phrase);
    }

    public interface IScenarioMatcher
    {
        MatchResult Match(string phrase);
    }

    public class CommandMatcher : ICommandMatcher
    {
        private readonly Dictionary<string, Command> _exactMatches = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);
        private readonly LevenshteinMatcher _levenshteinMatcher;
        private readonly TfIdfMatcher _tfidfMatcher = new TfIdfMatcher();

        public CommandMatcher(Dictionary<string, Command> commands)
        {
            foreach (var kvp in commands)
            {
                _exactMatches[kvp.Key] = kvp.Value;
            }
            _levenshteinMatcher = new LevenshteinMatcher(commands);
            _tfidfMatcher.AddCommands(commands.Values);
        }

        public MatchResult Match(string phrase)
        {
            var normalized = TextNormalizer.Normalize(phrase);

            if (_exactMatches.TryGetValue(normalized, out var exactCommand))
                return new MatchResult(exactCommand, 1.0f);

            if (normalized.Length < 15)
            {
                var levResult = _levenshteinMatcher.Match(normalized);
                if (levResult.Confidence > 0.8f)
                    return levResult;
            }

            var tfidfResult = _tfidfMatcher.Match(normalized);
            if (tfidfResult.Confidence > 0.4f)
                return tfidfResult;

            return new MatchResult(new Command(), 0f);
        }
    }

    public class LevenshteinMatcher : ILevenshteinMatcher
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
    public interface ILevenshteinMatcher
    {
        MatchResult MatchScenario(string input);
    }

}
