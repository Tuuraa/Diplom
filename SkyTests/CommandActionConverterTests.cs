using System.Text.Json;
using WPFComponents.Model.Interfaces;
using WPFComponents.Model.Utils;
using WPFComponents.Model.Commands;
using Xunit;
using SkyUtils;

namespace SkyTests
{
    public class CommandActionConverterTests
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            Converters = { new CommandActionConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        [Fact]
        public void Serialize_And_Deserialize_OpenAppCommand()
        {
            var command = new OpenAppCommand("C:\\Program Files\\MyApp.exe");
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options) as OpenAppCommand;

            Assert.NotNull(deserialized);
            Assert.Equal("C:\\Program Files\\MyApp.exe", deserialized.PathToExe);
        }

        [Fact]
        public void Serialize_And_Deserialize_MoveMouseCommand()
        {
            var command = new MoveMouseCommand(100, 200, true, "MoveMouseCommand");
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options) as MoveMouseCommand;

            Assert.NotNull(deserialized);
            Assert.Equal(100, deserialized.X);
            Assert.Equal(200, deserialized.Y);
            Assert.True(deserialized.Click);
        }

        [Fact]
        public void Serialize_And_Deserialize_PressKeyCommand()
        {
            var command = new PressKeyCommand("Enter", "PressKeyCommand");
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options) as PressKeyCommand;

            Assert.NotNull(deserialized);
            Assert.Equal("Enter", deserialized.Key);
        }

        [Fact]
        public void Deserialize_UnknownCommand_Throws()
        {
            string json = "{\"commandType\":\"UnknownCommand\"}";
            Assert.Throws<Exception>(() => JsonSerializer.Deserialize<ICommandAction>(json, _options));
        }

        [Fact]
        public void Serialize_And_Deserialize_DrawCircleCommand()
        {
            var command = new DrawCircleCommand(42, "DrawCircleCommand");
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options) as DrawCircleCommand;

            Assert.NotNull(deserialized);
            Assert.Equal(42, deserialized.Radius);
        }

        [Fact]
        public void Serialize_And_Deserialize_OpenSiteCommand()
        {
            var command = new OpenSiteCommand("https://example.com");
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options) as OpenSiteCommand;

            Assert.NotNull(deserialized);
            Assert.Equal("https://example.com", deserialized.Url);
        }

        [Fact]
        public void Serialize_And_Deserialize_PrintWordCommand()
        {
            var command = new PrintWordCommand("Hello");
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options) as PrintWordCommand;

            Assert.NotNull(deserialized);
            Assert.Equal("Hello", deserialized.Word);
        }

        [Fact]
        public void Serialize_And_Deserialize_DrawSquareCommand()
        {
            var command = new DrawSquareCommand(55, "DrawSquareCommand");
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options) as DrawSquareCommand;

            Assert.NotNull(deserialized);
            Assert.Equal(55, deserialized.SideLength);
        }

        [Fact]
        public void Serialize_And_Deserialize_HideWindowCommand()
        {
            var command = new HideWindowCommand();
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options);

            Assert.NotNull(deserialized);
            Assert.IsType<HideWindowCommand>(deserialized);
        }

        [Fact]
        public void Serialize_And_Deserialize_ScreenshotCommand()
        {
            var command = new ScrennShotCommand();
            var json = JsonSerializer.Serialize<ICommandAction>(command, _options);

            var deserialized = JsonSerializer.Deserialize<ICommandAction>(json, _options);

            Assert.NotNull(deserialized);
            Assert.IsType<ScrennShotCommand>(deserialized);
        }
    }
}