using Nodify.Calculator;
using SkyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFComponents.Model.Commands;
using CommandType = SkyUtils.CommandType;

namespace WPFComponents.Model.Utils
{
    public class CommandFactory
    {
        public static ICommandAction CreateCommand(CommandType commandType, OperationViewModel operation)
        {
            switch (commandType)
            {
                case CommandType.ButtonPress:
                    return new PressKeyCommand("K", "PressKeyCommand");
                case CommandType.HideWindow:
                    return new HideWindowCommand();
                case CommandType.MouseMove:
                    return new MoveMouseCommand(100,100,false, "MoveMouseCommand");
                case CommandType.OpenSite:
                    return new OpenSiteCommand(operation.Parametr);
                case CommandType.OpenApp:
                    return new OpenAppCommand(operation.Parametr); 
                case CommandType.ScreenShot:
                    return new ScrennShotCommand();
                default:
                    throw new ArgumentException("Invalid command type");
            }
        }
    }
}
