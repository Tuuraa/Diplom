using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace WPFComponents.Model.Utils
{
    class WebsocketMessageController<T>
    {
        private Dictionary<string, Action<T>> _messageHandlers;
        public FrozenDictionary<string, Action<T>> MessageHandlers => _messageHandlers.ToFrozenDictionary();
        public Action<T> DefaultAction { get; }

        public WebsocketMessageController(Action<T> defaultAction)
        {
            _messageHandlers = new Dictionary<string, Action<T>>();
            DefaultAction = defaultAction;
        }

        public void RegisterMessageHandler(Dictionary<string, Action<T>> messageHandlers)
        {
            foreach (var handler in messageHandlers)
            {
                RegisterMessageHandler(handler.Key, handler.Value);
            }
        }

        private void RegisterMessageHandler(string name, Action<T> action)
        {
            if (!_messageHandlers.ContainsKey(name)) 
            {
                _messageHandlers[name] = action;
            }
        }

        public void ExecuteAction(string name, T param)
        {
            if (_messageHandlers.TryGetValue(name, out var action))
            {
                action.Invoke(param);
            }
            else
            {
                DefaultAction.Invoke(param);
            }
        }
    }
}
