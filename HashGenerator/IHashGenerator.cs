﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace HashGenerator
{
    public interface IHashGenerator
    {
        Task<List<HashPair>> FromDirectoryAsync(string directory, string pattern = "*");
        Task<HashPair> FromFileAsync(string target);
    }
}
