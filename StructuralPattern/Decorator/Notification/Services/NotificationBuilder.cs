using DesignPattern.StructuralPattern.Decorator.Notification.Enums;
using DesignPattern.StructuralPattern.Decorator.Notification.Services.Decorator;

namespace DesignPattern.StructuralPattern.Decorator.Notification.Services;

public class NotificationBuilder
{
    private readonly List<ChannelConfiguration> _channelConfigurations = [];

    public NotificationBuilder AddChannel(NotificationEnums channel, Action<ChannelConfiguration>? configuration = null)
    {
        var config = new ChannelConfiguration(channel);
        configuration?.Invoke(config);
        _channelConfigurations.Add(config);
        return this;
    }
    
    public NotificationBuilder AddEmail(Action<ChannelConfiguration>? configure = null)
        => AddChannel(NotificationEnums.Email, configure);

    public NotificationBuilder AddFacebook(Action<ChannelConfiguration>? configure = null)
        => AddChannel(NotificationEnums.Facebook, configure);

    public NotificationBuilder AddSms(Action<ChannelConfiguration>? configure = null)
        => AddChannel(NotificationEnums.Sms, configure);
    
    
    /// <summary>
    /// Build INotification với tất cả decorators
    /// </summary>
    public INotification Build()
    {
        if (_channelConfigurations.Count == 0)
            throw new InvalidOperationException("Phải thêm ít nhất 1 channel");

        // Nếu chỉ 1 channel
        if (_channelConfigurations.Count == 1)
        {
            return BuildSingleChannel(_channelConfigurations[0]);
        }

        // Nếu nhiều channels → dùng Composite
        var composite = new CompositeNotification();
        foreach (var config in _channelConfigurations)
        {
            composite.Add(BuildSingleChannel(config));
        }
        return composite;
    }
    
    
    private INotification BuildSingleChannel(ChannelConfiguration config)
    {
        // 1. Tạo base channel
        INotification notification = CreateBaseChannel(config.Channel);

        // 2. Áp dụng decorators theo thứ tự: Encryption → Logging
        // (Thứ tự quan trọng: Logging ngoài cùng để log tất cả)

        if (!string.IsNullOrEmpty(config.EncryptionType))
        {
            notification = ApplyEncryption(notification, config.EncryptionType);
        }

        if (config.EnableLogging)
        {
            notification = ApplyLogging(notification);
        }

        return notification;
    }

    /// <summary>
    /// Tạo base notification channel
    /// </summary>
    private static INotification CreateBaseChannel(NotificationEnums channel)
    {
        return channel switch
        {
            NotificationEnums.Email    => new EmailNotification(),
            NotificationEnums.Facebook => new FacebookNotification(),
            NotificationEnums.Sms      => new SmsNotification(),
            _                            => throw new ArgumentException($"Unknown channel: {channel}")
        };
    }

    /// <summary>
    /// Áp dụng Encryption Decorator
    /// </summary>
    private INotification ApplyEncryption(INotification notification, string encryptionType)
    {
        return new EncryptionDecorator(notification, encryptionType);
    }

    /// <summary>
    /// Áp dụng Logging Decorator - TẠO TRỰC TIẾP trong Builder
    /// </summary>
    private INotification ApplyLogging(INotification notification)
    {
        return new LoggingNotificationDecorator(notification);
    }

}