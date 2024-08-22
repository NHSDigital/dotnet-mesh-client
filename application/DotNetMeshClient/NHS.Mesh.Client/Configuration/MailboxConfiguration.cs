namespace NHS.MESH.Client.Configuration;

using System.Security.Cryptography.X509Certificates;

public class MailboxConfiguration
{

    public string? Password {get; set;}
    public string? SharedKey { get; set; }
    public X509Certificate2? Cert {get; set;}

}
