using Nodify;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.ViewModel
{
    public class StateViewModel
    {
        public string Name { get; set; }
        public Point Location { get; set; }
        public Size Size { get; set; }
        public bool IsEditable { get; set; }
        public bool IsActive { get; set; }
        public bool IsRenaming { get; set; }
        public Connector Anchor { get; set; }

    }
    public class TransitionViewModel
    {
        public Connector Source { get; set; }
        public Connector Target { get; set; }
        public bool IsActive { get; set; }
    }

}
