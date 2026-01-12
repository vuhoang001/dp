namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies.Decorator;

public abstract class NotificationDecorator(INotification notification) : INotification
{
    public abstract void Send(Notification payload);
}