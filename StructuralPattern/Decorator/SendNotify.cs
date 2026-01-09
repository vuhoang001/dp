namespace DesignPattern.StructuralPattern.Decorator;

public interface INotify
{
    public string Notify();
}

public class FacebookNotify : INotify
{
    public string Notify() => "facebook";
}

public class EmailNotify : INotify
{
    public string Notify() => "Email";
}

public class SmsNotify : INotify
{
    public string Notify() => "SMS";
}

public class DecoratorNotify(INotify notify, DecoratorNotify? core = null) : INotify
{
    public string Notify()
    {
        var coreResult = core?.Notify();
        return notify.Notify() + (string.IsNullOrEmpty(coreResult) ? "" : " | " + coreResult);
    }

    public DecoratorNotify Decorator(INotify notifyDecor)
    {
        return new DecoratorNotify(notifyDecor, this);
    }
}