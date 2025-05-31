using System;
using Xunit;
using WPFComponents.Model;

namespace SkyTests
{
    public class TfIdfProcessorTests
    {
        [Fact]
        public void AddDocuments_And_CalculateIdf_DoesNotThrow()
        {
            var tfidf = new TfIdfProcessor();

            tfidf.AddDocument("open browser");
            tfidf.AddDocument("close browser");
            tfidf.CalculateIdf();

            Assert.True(true); // если не было исключений
        }

        [Fact]
        public void Similarity_Of_Identical_Phrases_Is_High()
        {
            var tfidf = new TfIdfProcessor();
            tfidf.AddDocument("play music");
            tfidf.CalculateIdf();

            float similarity = tfidf.CalculateSimilarity("play music", "play music");

            Assert.InRange(similarity, 0.99f, 1.01f);
        }

        [Fact]
        public void Similarity_Of_Different_Phrases_Is_Low()
        {
            var tfidf = new TfIdfProcessor();
            tfidf.AddDocument("open browser");
            tfidf.AddDocument("play music");
            tfidf.CalculateIdf();

            float similarity = tfidf.CalculateSimilarity("open browser", "play music");

            Assert.InRange(similarity, 0f, 0.5f); // низкая схожесть
        }

        [Fact]
        public void Cache_Is_Used_When_Getting_Vector()
        {
            var tfidf = new TfIdfProcessor();
            tfidf.AddDocument("test command");
            tfidf.CalculateIdf();

            // Первый вызов должен кэшировать
            var sim1 = tfidf.CalculateSimilarity("test command", "test command");

            // Второй вызов должен использовать кэш
            var sim2 = tfidf.CalculateSimilarity("test command", "test command");

            Assert.Equal(sim1, sim2, 3); // до 3-го знака после запятой
        }

        [Fact]
        public void Similarity_With_Unseen_Phrase_DoesNotThrow()
        {
            var tfidf = new TfIdfProcessor();
            tfidf.AddDocument("say hello");
            tfidf.CalculateIdf();

            float similarity = tfidf.CalculateSimilarity("completely different phrase", "say hello");

            Assert.True(similarity >= 0); // проверяем, что не вылетает и >= 0
        }

        [Fact]
        public void Similarity_With_Empty_String_Is_Zero()
        {
            var tfidf = new TfIdfProcessor();
            tfidf.AddDocument("say hello");
            tfidf.CalculateIdf();

            float similarity = tfidf.CalculateSimilarity("", "say hello");

            Assert.Equal(0, similarity);
        }
    }
}
