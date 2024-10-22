using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Command> Commands { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=DataBase.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Определяем модель
            modelBuilder.Entity<Command>()
                .Property(c => c.Action)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // сериализация ICommandAction
                    v => JsonSerializer.Deserialize<ICommandAction>(v, new JsonSerializerOptions())! // десериализация
                );
        }
    }
}
