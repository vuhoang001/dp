namespace DesignPattern.StructuralPattern.Decorator.Notification;

public class Notification
{
    public int Id { get; set; }
    public string To { get; set; } = string.Empty;
    public string From { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}