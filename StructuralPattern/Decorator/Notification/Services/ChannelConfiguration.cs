using DesignPattern.StructuralPattern.Decorator.Notification.Enums;

namespace DesignPattern.StructuralPattern.Decorator.Notification.Services;

public class ChannelConfiguration(NotificationEnums channel)
{
    public NotificationEnums Channel { get;  } = channel;
    public bool EnableLogging { get; private set; }
    public string? EncryptionType { get; private set; }
    public int? RetryCount { get; private set; }

    public ChannelConfiguration WithLogging()
    {
        EnableLogging = true;
        return this;
    }

    public ChannelConfiguration WithEncryption(string encryptionType)
    {
       EncryptionType =  encryptionType;
       return this;
    }

    public ChannelConfiguration WithRetryCount(int retryCount)
    {
        RetryCount = retryCount;
        return this;
    }
    
}