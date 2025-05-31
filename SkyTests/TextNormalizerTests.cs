using WPFComponents.Model;

namespace SkyTests
{
    public class TextNormalizerTests
    {
        [Theory]
        [InlineData("Привет, мир!", "привет  мир ")]
        [InlineData("HELLO, WORLD!", "hello  world ")]
        [InlineData("123#test!", "123 test ")]
        public void Normalize_ShouldRemovePunctuation_AndLowercase(string input, string expected)
        {
            var result = TextNormalizer.Normalize(input);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Привет, мир!", new[] { "привет", "мир" })]
        [InlineData("HELLO WORLD", new[] { "hello", "world" })]
        [InlineData("123#TEST!", new[] { "123", "test" })]
        [InlineData("   ", new string[] { })]
        [InlineData("!!!", new string[] { })]
        public void Tokenize_ShouldReturnExpectedTokens(string input, string[] expectedTokens)
        {
            var result = TextNormalizer.Tokenize(input);

            Assert.Equal(expectedTokens, result);
        }
    }

}
