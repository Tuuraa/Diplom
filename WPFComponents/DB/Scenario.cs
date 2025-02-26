using NAudio.Mixer;
using SkyUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.DB
{
    public class Scenario
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<string> Phrases { get; set; } = null!;
        public List<Command> Commands { get; set; } = null!;
    }
}
