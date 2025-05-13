using Microsoft.EntityFrameworkCore;
using SkyUtils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WPFComponents.DB;
using WPFComponents.Model;
using WPFComponents.Model.Commands;
using WPFComponents.Services;
using Xunit;
using Scenario = WPFComponents.DB.Scenario;

public class VoiceCommandProcessorTests
{
    [Fact]
    public async Task ProcessVoiceCommand_ExecutesMatchingCommand()
    {
        // Arrange
        var phrase = "открой браузер";
        var fakeAction = new FakeCommandAction();

        var command = new Command
        {
            Name = "Открыть браузер",
            Phrases = new List<string> { phrase },
            Action = fakeAction
        };

        var context = new FakeAppContext
        {
            CommandsList = new List<Command> { command },
            ScenariosList = new List<Scenario>()
        };

        var logger = new FakeLoggerService();
        var llmService = new FakeLLMActionService();

        var processor = new VoiceCommandProcessor(logger, llmService, context);

        // Act
        await processor.ProcessVoiceCommand(phrase);

        // Assert
        Assert.True(fakeAction.WasExecuted);
        Assert.Contains("Открыть браузер", logger.LoggedCommands);
    }

    [Fact]
    public async Task ProcessVoiceCommand_ExecutesScenarioIfConfidenceHigh()
    {
        // Arrange
        var phrase = "запусти рабочий день";

        var fakeAction1 = new FakeCommandAction();
        var fakeAction2 = new FakeCommandAction();

        var scenario = new Scenario
        {
            Name = "Рабочий день",
            Phrases = new List<string> { phrase },
            Commands = new List<Command>
            {
                new Command { Name = "Открыть Teams", Action = fakeAction1 },
                new Command { Name = "Открыть браузер", Action = fakeAction2 },
            }
        };

        var context = new FakeAppContext
        {
            CommandsList = new List<Command>(),
            ScenariosList = new List<Scenario> { scenario }
        };

        var logger = new FakeLoggerService();
        var llmService = new FakeLLMActionService();

        var processor = new VoiceCommandProcessor(logger, llmService, context);

        // Act
        await processor.ProcessVoiceCommand(phrase);

        // Assert
        Assert.True(fakeAction1.WasExecuted);
        Assert.True(fakeAction2.WasExecuted);
        Assert.Contains("Вызов сценарияРабочий день", logger.LoggedCommands);
    }

    public class FakeAppContext : ApplicationContext
    {
        public List<Command> CommandsList { get; set; } = new();
        public List<WPFComponents.DB.Scenario> ScenariosList { get; set; } = new();

        public override DbSet<Command> Commands => GetFakeDbSet(CommandsList);
        public override DbSet<Scenario> Scenarios => GetFakeDbSet(ScenariosList);

        private static DbSet<T> GetFakeDbSet<T>(List<T> list) where T : class
        {
            return new TestDbSet<T>(list);
        }
    }
    public class FakeCommandAction : ICommandAction
    {
        public bool WasExecuted { get; private set; }

        public bool CanExecute() => true;

        public Task Execute()
        {
            WasExecuted = true;
            return Task.CompletedTask;
        }
    }
    public class TestDbSet<T> : DbSet<T>, IQueryable<T> where T : class
    {
        private readonly IQueryable<T> _queryable;

        public TestDbSet(IEnumerable<T> data)
        {
            _queryable = data.AsQueryable();
        }

        public override Type ElementType => _queryable.ElementType;
        public override Expression Expression => _queryable.Expression;
        public override IQueryProvider Provider => _queryable.Provider;
    }
    public class FakeLoggerService : LoggerService
    {
        public List<string> LoggedCommands { get; } = new();

        public override void LogCommand(string text)
        {
            LoggedCommands.Add(text);
        }
    }

    public class FakeLLMActionService : LLMActionService
    {
        public override Task<string> GenerateCodeAsync(string phrase) => Task.FromResult("fake code");
        public override Task ExecuteGeneratedCodeAsync(string code) => Task.CompletedTask;
    }


}
