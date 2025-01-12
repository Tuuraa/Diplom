using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodify;

namespace WPFComponents.Model
{
    public class StateNode
    {
        public string Name { get; set; } = "State";

        public ObservableCollection<StateTransition> Transitions { get; set; } = new();
    }

    public class StateTransition 
    {
        public string Condition { get; set; } = "Condition";
    }

}
