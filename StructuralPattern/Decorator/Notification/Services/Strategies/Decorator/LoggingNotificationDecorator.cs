namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies.Decorator;

public class LoggingNotificationDecorator(INotification notification) : NotificationDecorator(notification)
{
   private readonly INotification _notification = notification;

   public override void Send(Notification payload)
   {
      Console.WriteLine($"[LOG] Sending notification to {payload.To}");
      _notification.Send(payload);
   }
}