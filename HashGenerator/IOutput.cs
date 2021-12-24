namespace HashGenerator;

internal interface IOutput
{
    Task Write(HashPair pair);
}
