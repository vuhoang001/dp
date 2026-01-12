using DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies;
using DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies.Decorator;

namespace DesignPattern.StructuralPattern.Decorator.Notification.Services;

public static class NotificationDecoratorFactory
{
    public static INotification Apply(INotification notification, ChannelConfiguration config)
    {
        if (!string.IsNullOrEmpty(config.EncryptionType))
        {
            notification = new EncryptionDecorator(notification, config.EncryptionType);
        }

        if (config.EnableLogging)
        {
            notification = new LoggingNotificationDecorator(notification);
        }

        return notification;
    }
}