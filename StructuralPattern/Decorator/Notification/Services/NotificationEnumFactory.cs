using DesignPattern.StructuralPattern.Decorator.Notification.Enums;
using DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies;

namespace DesignPattern.StructuralPattern.Decorator.Notification.Services;

public static class NotificationEnumFactory
{
    public static INotification Create(NotificationEnums enums)
    {
        return enums switch
        {
            NotificationEnums.Facebook => new EmailNotification(),
            NotificationEnums.Sms      => new SmsNotification(),
            NotificationEnums.Email    => new EmailNotification(),
            _                          => throw new Exception()
        };
    }
    
}