namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies;

public class SmsNotification : INotification
{
    public void Send(Notification notification)
    {
        Console.WriteLine("Send Sms Notification");
    }
}