using Microsoft.EntityFrameworkCore;
using SkyUtils;
using System.Text.Json;
using WPFComponents.Model.Interfaces;
using WPFComponents.Model.Utils;

namespace WPFComponents.DB
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Command> Commands { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<LogEntry> Logs { get; set; }
        public DbSet<InstalledProgram> InstalledPrograms { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
        }
        public ApplicationContext() : base(new DbContextOptions<ApplicationContext>()) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured) // Если контекст создаётся без DI
            {
                optionsBuilder.UseSqlite("Data Source=database.db");
            }
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new CommandActionConverter() }, // Ваш кастомный конвертер
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Политика именования
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
        }
    }
}
