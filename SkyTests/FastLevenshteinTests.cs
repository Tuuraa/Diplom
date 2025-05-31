using Xunit;
using WPFComponents.Model;

namespace SkyTests
{
    public class FastLevenshteinTests
    {
        [Theory]
        [InlineData("test", "test", 1.0f)]
        [InlineData("test", "tent", 0.75f)]
        [InlineData("kitten", "sitting", 0.5714286f)]
        [InlineData("abc", "def", 0.0f)]
        [InlineData("a", "a", 1.0f)]
        [InlineData("a", "", 0.0f)]
        [InlineData("", "", 0.0f)]
        public void GetSimilarity_ShouldReturnExpectedValue(string a, string b, float expected)
        {
            float result = FastLevenshtein.GetSimilarity(a, b);

            Assert.Equal(expected, result, 4);
        }

        [Fact]
        public void GetSimilarity_ShouldBeSymmetric()
        {
            string a = "hello";
            string b = "hullo";

            float sim1 = FastLevenshtein.GetSimilarity(a, b);
            float sim2 = FastLevenshtein.GetSimilarity(b, a);

            Assert.Equal(sim1, sim2, 4);
        }

        [Fact]
        public void GetSimilarity_ShouldUseCache_OnRepeatedCalls()
        {
            string a = "repeat";
            string b = "repeal";

            float sim1 = FastLevenshtein.GetSimilarity(a, b);

            float sim2 = FastLevenshtein.GetSimilarity(a, b);

            Assert.Equal(sim1, sim2, 4);
        }
    }
}