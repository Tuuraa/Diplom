using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    public class LogEntry
    {
        public int Id { get; set; } 

        public DateTime Timestamp { get; set; } 
        public string Command { get; set; }     
        public string Response { get; set; }   
        public string Result { get; set; }      
    }
}
