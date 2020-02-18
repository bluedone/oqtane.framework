﻿using Oqtane.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oqtane.Services
{
    public interface IAliasService
    {
        Task<List<Alias>> GetAliasesAsync();

        Task<Alias> GetAliasAsync(int AliasId);

        Task<Alias> GetAliasAsync(string Url);

        Task<Alias> AddAliasAsync(Alias Alias);

        Task<Alias> UpdateAliasAsync(Alias Alias);

        Task DeleteAliasAsync(int AliasId);
    }
}
