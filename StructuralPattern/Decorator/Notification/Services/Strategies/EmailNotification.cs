namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies;

public class EmailNotification : INotification
{
    public void Send(Notification notification)
    {
        Console.WriteLine("Send Email Notification");
    }
}