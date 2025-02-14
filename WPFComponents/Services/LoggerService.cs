using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFComponents.DB;

namespace WPFComponents.Services
{
    public class LoggerService
    {
        private readonly ApplicationContext _context;

        public LoggerService(ApplicationContext context)
        {
            _context = new ApplicationContext();
        }

        public LoggerService()
        {

        }

        public void LogCommand(string commandText)
        {
            var log = new LogEntry { Command = commandText };
            _context.Logs.Add(log);
            _context.SaveChanges();
        }
        public List<LogEntry> GetLogs()
        {
            return _context.Logs.OrderByDescending(l => l.Timestamp).ToList();
        }

    }
}
