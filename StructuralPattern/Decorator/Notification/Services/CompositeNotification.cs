namespace DesignPattern.StructuralPattern.Decorator.Notification.Services;

public class CompositeNotification : INotification
{
    private readonly List<INotification> _notifications = [];

    public void Add(INotification notification)
    {
        _notifications.Add(notification);
    }
    public void Send(Notification notification)
    {
        foreach (var notification1 in _notifications)
        {
           notification1.Send(notification);
        }
    }
}