using System.Collections.Generic;
using System.Threading.Tasks;

namespace HashGenerator
{
    public interface IOutput
    {
        Task Write(List<HashPair> hashes);
    }
}
