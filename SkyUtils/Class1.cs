namespace SkyUtils
{
    public class Command
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<string> Phrases { get; set; } = null!;
        public ICommandAction Action { get; set; } = null!;
        public string Type { get; set; } = null!;
    }
    public interface ICommandAction
    {
        public bool CanExecute();
        public Task Execute();
    }
}
