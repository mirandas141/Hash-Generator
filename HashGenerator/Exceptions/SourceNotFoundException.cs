namespace HashGenerator.Exceptions;

[Serializable]
internal class SourceNotFoundException : Exception
{
    public SourceNotFoundException() { }
    public SourceNotFoundException(string source) : base($"Unable to locate source: '{source}'.") { }
    public SourceNotFoundException(string source, Exception inner) : base(source, inner) { }
    protected SourceNotFoundException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}


[Serializable]
public class NoOutputEnabledException : Exception
{
    public NoOutputEnabledException() 
        : base("Can not silence console output without specifying target") { }

    protected NoOutputEnabledException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
