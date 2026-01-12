namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Decorator;

public class NotificationDecorator(INotification notification) : INotification
{
    public virtual void Send(Notification payload)
    {
        notification.Send(payload);
    }
}