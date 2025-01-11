using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFComponents.Model.Utils
{
    public interface ILoggerService
    {
        Task LogToDatabaseAsync(string command, string response, string result);
    }
    public class LoggerService : ILoggerService
    {
        private readonly ApplicationContext _context;

        public LoggerService(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task LogToDatabaseAsync(string command, string response, string result)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command cannot be empty.", nameof(command));

            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Command = command,
                Response = response,
                Result = result
            };

            _context.Logs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
    }
}
