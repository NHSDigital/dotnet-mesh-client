namespace NHS.MESH.Client.Configuration;

public class MailboxConfigurationResolver
{
    private Dictionary<string,MailboxConfiguration> _mailboxes;

    public MailboxConfigurationResolver(Dictionary<string,MailboxConfiguration> mailboxes)
    {
        _mailboxes = mailboxes;
    }

    public MailboxConfiguration GetMailboxConfiguration(string mailboxId)
    {
        if(!_mailboxes.TryGetValue(mailboxId,out MailboxConfiguration mailboxConfiguration))
        {
            throw new KeyNotFoundException($"MailboxId: {mailboxId} has not been configured");
        }
        return mailboxConfiguration;
    }
}
