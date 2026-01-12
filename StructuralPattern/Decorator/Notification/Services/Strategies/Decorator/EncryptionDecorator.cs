namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies.Decorator;

public class EncryptionDecorator(INotification notification, string type) : NotificationDecorator(notification)
{
    private readonly INotification _notification = notification;

    public override void Send(Notification payload)
    {
        Console.WriteLine($"[ENCRYPTION] : Encryption with type :{type}", type);
        _notification.Send(payload);
    }
}