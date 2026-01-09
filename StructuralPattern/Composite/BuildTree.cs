namespace DesignPattern.StructuralPattern.Composite;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
}

public interface ITreeNode
{
    public int Id { get; }
    public string Name { get; }

    void Display(int level);
}
//
// public class LeftNode(int id, string name) : ITreeNode
// {
//     public int Id { get; } = id;
//     public string Name { get; } = name;
//
//     public void Display(int level)
//     {
//         Console.WriteLine($"{Name} - {level}");
//     }
// }

public class CompositeNode(int id, string name) : ITreeNode
{
    public int Id { get; } = id;
    public string Name { get; } = name;

    private readonly List<ITreeNode> childrens = [];

    public void Add(ITreeNode child)
    {
        childrens.Add(child);
    }

    public void Remove(ITreeNode child)
    {
        childrens.Remove(child);
    }

    public void Display(int level)
    {
        Console.WriteLine($"{new string('-', level)} {Name}");

        foreach (var child in childrens)
        {
            child.Display(level + 2);
        }
    }
}