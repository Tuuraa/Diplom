using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model.Interfaces
{
    public interface ICommandAction
    {
        public bool CanExecute();
        public void Execute();
    }
}
