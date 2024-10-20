using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model
{
    internal abstract class CommandBase : ICommandAction
    {
        public abstract bool CanExecute();

        public abstract void Execute();

        public string SerializeCommand() => 
            JsonSerializer.Serialize(this, this.GetType());

        public CommandBase? DeserializeCommand(string json, Type commandType) =>
            JsonSerializer.Deserialize(json, commandType) as CommandBase;

    }
}
