using DesignPattern.StructuralPattern.Decorator.Notification;
using DesignPattern.StructuralPattern.Decorator.Notification.Enums;
using DesignPattern.StructuralPattern.Decorator.Notification.Services;

var notification = new Notification
{
    Id          = 1,
    To          = "To",
    From        = "From",
    Title       = "Title",
    Description = "Description"
};


// var emailService = new EmailNotification()
//     .AddEncryption("SHA250")
//     .AddLogging();
//
// var smsService = new SmsNotification().AddEncryption("SHA255");
// var facebookService = new FacebookNotification().AddLogging();
//
// var composite = new CompositeNotification();
// composite.Add(emailService);
// composite.Add(facebookService);
// composite.Add(smsService);
//
// composite.Send(notification);


var builder = new NotificationBuilder()
    .AddChannel(NotificationEnums.Sms, configuration =>
    {
        configuration.WithLogging();
        configuration.WithEncryption("123123");
    } )
    .AddEmail(config =>
    {
        config.WithLogging();
        config.WithEncryption("SHA3636");
    })
    .AddFacebook()
    .AddSms();
    
var result =     builder.Build();
result.Send(notification);