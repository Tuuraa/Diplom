using Nodify.Calculator;
using Nodify.Calculator.Commands;
using SkyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Web.Syndication;
using WPFComponents.Model.Commands;
using CommandType = SkyUtils.CommandType;

namespace WPFComponents.Model.Utils
{
    public class CommandFactory
    {
        public static ICommandAction CreateCommand(CommandType commandType, OperationViewModel operation)
        {
            switch (operation.CommandType)
            {
                case CommandType.ButtonPress:
                    return new PressKeyCommand("K", "PressKeyCommand");
                case CommandType.HideWindow:
                    return new HideWindowCommand();
                case CommandType.MouseMove:
                    var mouseMoveOperation = operation as MouseMoveViewModel;
                    if (mouseMoveOperation == null)
                    {
                        throw new InvalidOperationException("Operation is not of type MouseMoveViewModel.");
                    }
                    return new MoveMouseCommand(mouseMoveOperation.X, mouseMoveOperation.Y, mouseMoveOperation.Click, "MoveMouseCommand");
                case CommandType.OpenSite:
                    var openSiteOperation = operation as OpenSiteViewModel;
                    if(openSiteOperation == null)
                    {
                        throw new InvalidOperationException("a");
                    }
                    return new OpenSiteCommand(openSiteOperation.Url);
                case CommandType.OpenApp:
                    return new OpenAppCommand(operation.Parametr);
                case CommandType.ScreenShot:
                    return new ScrennShotCommand();
                case CommandType.DrawCircle:
                    var circle = operation as DrawCircleViewModel;
                    if(circle == null)
                    {
                        throw new InvalidOperationException("Operation is not of type DrawCircle.");
                    }
                    return new DrawCircleCommand(circle.Radius, "DrawCircleCommand");
                default:
                    throw new ArgumentException("Invalid command type");
            }
        }
    }
}
