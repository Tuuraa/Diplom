using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model
{
    internal class CommandRegistry
    {
        private static readonly Dictionary<string, ICommandAction> _commandTypes 
            = new Dictionary<string, ICommandAction>();

        public static void Add(string name, ICommandAction command)
        {
            if (_commandTypes.ContainsKey(name))
            {
                return;
            }

            _commandTypes[name] = command;
        }

        public static ICommandAction GetCommand(string name)
        {
            return _commandTypes.TryGetValue(name, out var command)
                ? command
                : throw new ArgumentException($"Command '{name}' not found.");
        }

        public static IEnumerable<string> GetCommandNames()
        {
            return _commandTypes.Keys;
        }
    }
}
