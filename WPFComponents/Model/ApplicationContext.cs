using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WPFComponents.Model.Interfaces;
using WPFComponents.Model.Utils;

namespace WPFComponents.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<LogEntry> Logs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\DiplomUI\WPFComponents\DataBase.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new CommandActionConverter() }, 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };


            modelBuilder.Entity<Scenario>()
                .Property(s => s.Phrases)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())!
                );

            modelBuilder.Entity<Command>()
                    .Property(c => c.Action)
                    .HasConversion(
                    v => JsonSerializer.Serialize(v, options), // сериализация ICommandAction
                    v => JsonSerializer.Deserialize<ICommandAction>(v, options)! // десериализация
                );

            modelBuilder.Entity<Scenario>()
                .HasMany(s => s.Commands)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LogEntry>().ToTable("Logs");

        }
    }
}
