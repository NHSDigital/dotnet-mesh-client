namespace NHS.MESH.Client;

[System.Serializable]
public class MailboxNotAuthenticatedException : System.Exception
{
  public MailboxNotAuthenticatedException() { }
  public MailboxNotAuthenticatedException(string message) : base(message) { }
  public MailboxNotAuthenticatedException(string message, System.Exception inner) : base(message, inner) { }
  protected MailboxNotAuthenticatedException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
