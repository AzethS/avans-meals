using Microsoft.AspNetCore.Identity.UI.Services;

namespace AvansMeals.Web.Services;

public class DummyEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // geen echte mail versturen
        return Task.CompletedTask;
    }
}
