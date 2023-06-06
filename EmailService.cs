namespace OhfsFileEngineMonitor;
using SendGrid;
using SendGrid.Helpers.Mail;

public sealed class EmailService
{
    private readonly ISendGridClient _sendGridClient ;
    private readonly IConfiguration _configuration;
    private readonly EmailAddress fromAddr;
    private readonly List<EmailAddress> recipients= new List<EmailAddress>(); 
    public EmailService(ISendGridClient sendGridClient, IConfiguration configuration)
    {
        _sendGridClient = sendGridClient;
        _configuration = configuration;
        fromAddr = new EmailAddress(_configuration.GetValue<string>("EmailSender"));
        foreach(var recipient in  _configuration.GetValue<string>("EmailRecipients").Split(","))
        {
            recipients.Add(new EmailAddress(recipient));
        }
    }

    public async Task SendEmail(string subject, string content)
    {
        SendGridMessage sm = new SendGridMessage();
        sm.From = fromAddr;
        sm.Subject = subject;
        sm.PlainTextContent = content;
        foreach(var recipient in recipients){
            sm.AddTo(recipient);
        }

        var response = await _sendGridClient.SendEmailAsync(sm).ConfigureAwait(false);
    }
}