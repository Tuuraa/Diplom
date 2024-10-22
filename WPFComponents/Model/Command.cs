using WPFComponents.Model.Interfaces;

namespace WPFComponents.Model;

public partial class Command
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Phrase { get; set; } = null!;

   public ICommandAction Action { get; set; } = null!;
}
