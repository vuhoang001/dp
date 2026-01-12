namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Decorator;

public class EncryptionDecorator(INotification notification, string type) : NotificationDecorator(notification)
{
    public override void Send(Notification payload)
    {
        Console.WriteLine($"[ENCRYPTION] : Encryption with type :{type}", type);
        base.Send(payload);
    }
}