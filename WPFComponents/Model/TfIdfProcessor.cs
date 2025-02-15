using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    public class TfIdfProcessor
    {
        private readonly Dictionary<string, Dictionary<string, float>> _tf = new();
        private readonly Dictionary<string, float> _idf = new();
        private readonly HashSet<string> _allTerms = new();
        private readonly LRUCache<string, float[]> _vectorCache;

        public TfIdfProcessor(int cacheSize = 100)
        {
            _vectorCache = new LRUCache<string, float[]>(cacheSize);
        }

        // Добавление документа (команды) в модель
        public void AddDocument(string document)
        {
            var normalized = TextNormalizer.Normalize(document);
            var terms = normalized.Split();

            // Вычисление TF
            var tf = new Dictionary<string, float>();
            foreach (var term in terms)
            {
                if (tf.ContainsKey(term)) tf[term] += 1;
                else tf[term] = 1;
                _allTerms.Add(term);
            }

            // Нормализация TF
            var maxTf = tf.Values.Max();
            foreach (var key in tf.Keys.ToList())
            {
                tf[key] /= maxTf;
            }

            _tf[document] = tf;
        }

        // Расчет IDF после добавления всех документов
        public void CalculateIdf()
        {
            int totalDocs = _tf.Count;
            foreach (var term in _allTerms)
            {
                int docsWithTerm = _tf.Count(d => d.Value.ContainsKey(term));
                _idf[term] = (float)Math.Log(totalDocs / (double)docsWithTerm);
            }
        }

        // Основной метод расчета схожести
        public float CalculateSimilarity(string phrase, string document)
        {
            var phraseVector = GetVector(phrase);
            var docVector = GetVector(document);
            return CosineSimilarity(phraseVector, docVector);
        }

        private float[] GetVector(string text)
        {
            if (_vectorCache.TryGetValue(text, out var cached))
                return cached;

            var vector = ComputeVector(text);
            _vectorCache.AddOrUpdate(text, vector);
            return vector;
        }

        private float[] ComputeVector(string text)
        {
            var normalized = TextNormalizer.Normalize(text);
            var terms = normalized.Split();

            var vector = new float[_allTerms.Count];
            int i = 0;

            foreach (var term in _allTerms)
            {
                // TF
                float tf = terms.Count(t => t == term) / (float)terms.Length;

                // IDF
                float idf = _idf.GetValueOrDefault(term, 0);

                vector[i++] = tf * idf;
            }

            return vector;
        }

        private static float CosineSimilarity(float[] a, float[] b)
        {
            float dot = 0, magA = 0, magB = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }
            return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
        }
    }
}
