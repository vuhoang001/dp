namespace DesignPattern.StructuralPattern.Decorator.Notification.Services;

public class EmailNotification : INotification
{
    public void Send(Notification notification)
    {
        Console.WriteLine("Send Email Notification");
    }
}