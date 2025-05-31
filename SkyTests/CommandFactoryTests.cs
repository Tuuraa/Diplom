//using Xunit;
//using WPFComponents.Model.Utils;
//using WPFComponents.Model.Commands;
//using SkyUtils;
//using Nodify.Calculator.Commands;
//using Nodify.Calculator;

//public class CommandFactoryTests
//{
//    [Fact]
//    public void CreateCommand_ButtonPress_ReturnsPressKeyCommand()
//    {
//        var command = CommandFactory.CreateCommand(CommandType.ButtonPress, new OperationViewModel());
//        Assert.IsType<PressKeyCommand>(command);
//    }

//    [Fact]
//    public void CreateCommand_MouseMove_ReturnsMoveMouseCommand()
//    {
//        var op = new MouseMoveViewModel { X = 10, Y = 20, Click = true };
//        var command = CommandFactory.CreateCommand(CommandType.MouseMove, op);
//        var moveCommand = Assert.IsType<MoveMouseCommand>(command);
//        Assert.Equal(10, moveCommand.X);
//        Assert.Equal(20, moveCommand.Y);
//        Assert.True(moveCommand.Click);
//    }

//    [Fact]
//    public void CreateCommand_OpenSite_ReturnsOpenSiteCommand()
//    {
//        var op = new OpenSiteViewModel { Url = "https://test.com" };
//        var command = CommandFactory.CreateCommand(CommandType.OpenSite, op);
//        var siteCommand = Assert.IsType<OpenSiteCommand>(command);
//        Assert.Equal("https://test.com", siteCommand.Url);
//    }

//    [Fact]
//    public void CreateCommand_OpenApp_ReturnsOpenAppCommand()
//    {
//        var op = new OperationViewModel { Parametr = "C:\\app.exe" };
//        var command = CommandFactory.CreateCommand(CommandType.OpenApp, op);
//        var appCommand = Assert.IsType<OpenAppCommand>(command);
//        Assert.Equal("C:\\app.exe", appCommand.PathToExe);
//    }

//    [Fact]
//    public void CreateCommand_InvalidOperation_Throws()
//    {
//        var op = new OperationViewModel();
//        Assert.Throws<InvalidOperationException>(() =>
//            CommandFactory.CreateCommand(CommandType.MouseMove, op));
//    }

//    [Fact]
//    public void CreateCommand_InvalidCommandType_Throws()
//    {
//        var op = new OperationViewModel();
//        Assert.Throws<ArgumentException>(() =>
//            CommandFactory.CreateCommand((CommandType)999, op));
//    }
//}
