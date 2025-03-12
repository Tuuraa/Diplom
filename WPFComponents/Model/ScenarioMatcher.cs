using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WPFComponents.Services;
using WPFComponents.DB;
using SkyUtils;
using Scenario = WPFComponents.DB.Scenario;

namespace WPFComponents.Model
{
    public class ScenarioMatcher : IScenarioMatcher
    {
        private readonly Dictionary<string, Scenario> _scenarios = new Dictionary<string, Scenario>(StringComparer.OrdinalIgnoreCase);
        private readonly LevenshteinMatcher _levenshteinMatcher;
        private readonly TfIdfMatcher _tfidfMatcher;

        public ScenarioMatcher(Dictionary<string, Scenario> scenarios)
        {
            // Преобразуем Dictionary в IEnumerable<Scenario>
            var scenarioList = scenarios.Values.ToList();

            foreach (var kvp in scenarios)
            {
                _scenarios[kvp.Key] = kvp.Value;
            }

            _levenshteinMatcher = new LevenshteinMatcher(scenarios); // Если LevenshteinMatcher принимает Dictionary
            _tfidfMatcher = new TfIdfMatcher();
            _tfidfMatcher.AddScenarios(scenarioList); // Передаем список сценариев вместо словаря
        }


        public MatchResult Match(string phrase)
        {
            var normalized = TextNormalizer.Normalize(phrase);

            // Этап 1: Точное совпадение
            if (_scenarios.TryGetValue(normalized, out var exactCommand))
                return new MatchResult(exactCommand, 1.0f);

            // Этап 2: Левенштейн для коротких фраз
            if (normalized.Length < 15)
            {
                var levResult = _levenshteinMatcher.MatchScenario(normalized);
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
}
