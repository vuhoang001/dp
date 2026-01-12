using DesignPattern.StructuralPattern.Decorator.Notification.Enums;
using DesignPattern.StructuralPattern.Decorator.Notification.Services.Strategies;

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
        switch (_channelConfigurations.Count)
        {
            case 0:
                throw new InvalidOperationException("Phải thêm ít nhất 1 channel");
            case 1:
                return BuildSingleChannel(_channelConfigurations[0]);
        }

        var composite = new CompositeNotification();
        foreach (var config in _channelConfigurations)
        {
            composite.Add(BuildSingleChannel(config));
        }

        return composite;
    }


    private static INotification BuildSingleChannel(ChannelConfiguration config)
    {
        var notification = NotificationEnumFactory.Create(config.Channel);

        notification = NotificationDecoratorFactory.Apply(notification, config);

        return notification;
    }


}