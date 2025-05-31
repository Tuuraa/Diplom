//using Xunit;
//using WPFComponents.Model;
//using System.Collections.Generic;
//using WPFComponents.DB;

//public class ScenarioMatcherTests
//{
//    private class FakeLevenshteinMatcher : LevenshteinMatcher
//    {
//        private readonly MatchResult _result;
//        public FakeLevenshteinMatcher(MatchResult result) : base(new()) => _result = result;
//        public override MatchResult MatchScenario(string input) => _result;
//    }

//    private class FakeTfIdfMatcher : TfIdfMatcher
//    {
//        private readonly MatchResult _result;
//        public FakeTfIdfMatcher(MatchResult result) => _result = result;
//        public override MatchResult Match(string input) => _result;
//    }

//    [Fact]
//    public void Match_ExactMatch_ShouldReturnConfidence1()
//    {
//        // Arrange
//        var scenario = new Scenario { Id = 1, Name = "Open browser" };
//        var scenarios = new Dictionary<string, Scenario>
//        {
//            { "open browser", scenario }
//        };

//        var matcher = new ScenarioMatcher(scenarios);

//        // Act
//        var result = matcher.Match("Open Browser");

//        // Assert
//        Assert.Equal(1.0f, result.Confidence, 4);
//        Assert.Equal(scenario.Id, result.Scenario.Id);
//    }

//    [Fact]
//    public void Match_LevenshteinMatch_ShouldReturnConfidenceAboveThreshold()
//    {
//        // Arrange
//        var expected = new Scenario { Id = 2, Name = "Play music" };
//        var fakeLevResult = new MatchResult(expected, 0.85f);

//        var matcher = new ScenarioMatcher(new Dictionary<string, Scenario>())
//        {
//            // подменяем внутреннее поле через отражение (или делаем через конструктор, если доступно)
//            _levenshteinMatcher = new FakeLevenshteinMatcher(fakeLevResult),
//            _tfidfMatcher = new FakeTfIdfMatcher(new MatchResult(new Scenario(), 0f)) // не должен использоваться
//        };

//        // Act
//        var result = matcher.Match("plai music");

//        // Assert
//        Assert.Equal(expected.Id, result.Scenario.Id);
//        Assert.Equal(0.85f, result.Confidence, 2);
//    }

//    [Fact]
//    public void Match_TfIdfMatch_ShouldReturnConfidenceAboveThreshold()
//    {
//        // Arrange
//        var expected = new Scenario { Id = 3, Name = "Send email" };
//        var fakeTfidfResult = new MatchResult(expected, 0.7f);

//        var matcher = new ScenarioMatcher(new Dictionary<string, Scenario>())
//        {
//            _levenshteinMatcher = new FakeLevenshteinMatcher(new MatchResult(new Scenario(), 0.1f)),
//            _tfidfMatcher = new FakeTfIdfMatcher(fakeTfidfResult)
//        };

//        // Act
//        var result = matcher.Match("send emale");

//        // Assert
//        Assert.Equal(expected.Id, result.Scenario.Id);
//        Assert.Equal(0.7f, result.Confidence, 2);
//    }

//    [Fact]
//    public void Match_NoMatchFound_ShouldReturnEmptyScenarioWithZeroConfidence()
//    {
//        // Arrange
//        var matcher = new ScenarioMatcher(new Dictionary<string, Scenario>())
//        {
//            _levenshteinMatcher = new FakeLevenshteinMatcher(new MatchResult(new Scenario(), 0.1f)),
//            _tfidfMatcher = new FakeTfIdfMatcher(new MatchResult(new Scenario(), 0.3f))
//        };

//        // Act
//        var result = matcher.Match("unknown phrase");

//        // Assert
//        Assert.Equal(0f, result.Confidence);
//        Assert.NotNull(result.Scenario);
//    }
//}
