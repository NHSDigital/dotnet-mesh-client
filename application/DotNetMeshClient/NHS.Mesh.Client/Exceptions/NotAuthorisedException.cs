[System.Serializable]
public class NotAuthorisedExceptionException : System.Exception
{
  public NotAuthorisedExceptionException() { }
  public NotAuthorisedExceptionException(string message) : base(message) { }
  public NotAuthorisedExceptionException(string message, System.Exception inner) : base(message, inner) { }
  protected NotAuthorisedExceptionException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
