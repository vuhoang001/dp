namespace DesignPattern.StructuralPattern.Decorator.Notification.Services.Decorator;

public class LoggingNotificationDecorator(INotification notification) : NotificationDecorator(notification)
{
   public override void Send(Notification payload)
   {
      Console.WriteLine($"[LOG] Sending notification to {payload.To}");
      base.Send(payload);
   }
}