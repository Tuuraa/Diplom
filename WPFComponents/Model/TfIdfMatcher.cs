using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Accord.MachineLearning;
using Accord.Math.Distances;
using SkyUtils;
using Scenario = WPFComponents.DB.Scenario;

namespace WPFComponents.Model
{
    public class TfIdfMatcher
    {
        private TFIDF _tfidf;
        private List<(Command Command, string[] Terms)> _commandDocuments = new();
        private List<(Scenario Scenario, string[] Terms)> _scenarioDocuments = new();
        private double[][] _documentVectors;
        private readonly Cosine _cosine = new Cosine();

        public void AddCommands(IEnumerable<Command> commands)
        {
            var documentsTerms = new List<string[]>();
            foreach (var cmd in commands)
            {
                var terms = TextNormalizer.Tokenize(cmd.Phrases.First());
                _commandDocuments.Add((cmd, terms));
                documentsTerms.Add(terms);
            }
            TrainModel(documentsTerms);
        }

        public void AddScenarios(IEnumerable<Scenario> scenarios)
        {
            var documentsTerms = new List<string[]>();
            foreach (var scenario in scenarios)
            {
                var terms = TextNormalizer.Tokenize(scenario.Name);
                _scenarioDocuments.Add((scenario, terms));
                documentsTerms.Add(terms);
            }
            TrainModel(documentsTerms);
        }

        private void TrainModel(List<string[]> documentsTerms)
        {
            if (documentsTerms.Count == 0) return;

            _tfidf = new TFIDF
            {
                Tf = TermFrequency.Log,
                Idf = InverseDocumentFrequency.Default
            };

            // Обучаем модель и сразу преобразуем документы в векторы
            _tfidf.Learn(documentsTerms.ToArray());
            _documentVectors = _tfidf.Transform(documentsTerms.ToArray());
        }

        public MatchResult Match(string query, bool isScenario = false)
        {
            if (_documentVectors == null || _documentVectors.Length == 0)
                return new MatchResult((Command)null, 0f);

            // Токенизация запроса
            var queryTerms = TextNormalizer.Tokenize(query);

            try
            {
                var queryVector = _tfidf.Transform(queryTerms);
                int bestIndex = -1;
                double maxSimilarity = 0;

                // Поиск лучшего совпадения
                for (int i = 0; i < _documentVectors.Length; i++)
                {
                    var similarity = 1 - _cosine.Distance(queryVector, _documentVectors[i]);
                    if (similarity > maxSimilarity)
                    {
                        maxSimilarity = similarity;
                        bestIndex = i;
                    }
                }

                if (bestIndex == -1) return new MatchResult((Command)null, 0f);

                // Выбор соответствующего результата
                return new MatchResult(_commandDocuments[bestIndex].Command, (float)maxSimilarity);
            }
            catch (Exception)
            {
                return new MatchResult((Command)null, 0f);
            }
        }
    }

    public static class TextNormalizer
    {
        private static readonly Regex _cleanRegex = new Regex("[^а-яa-z0-9 ]", RegexOptions.Compiled);

        public static string Normalize(string input)
        {
            return _cleanRegex.Replace(input.ToLowerInvariant(), " ");
        }

        public static string[] Tokenize(string input)
        {
            var normalized = Normalize(input);
            return normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}