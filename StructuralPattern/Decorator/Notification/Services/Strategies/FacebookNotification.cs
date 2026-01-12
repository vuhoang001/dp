namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies;

public class FacebookNotification : INotification
{
    public void Send(Notification notification)
    {
        Console.WriteLine("Send Facebook Notification");
    }
}