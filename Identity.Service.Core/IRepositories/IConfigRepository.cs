using Identity.Service.Core.Entities;
using System;

namespace Identity.Service.Core.IRepositories
{
    /// <summary>
    /// Handles only configuration lookups.
    /// </summary>
    public interface IConfigRepository
    {
        Task<Configuration> GetByKeyAsync(string key);
        Task<Configuration> GetByIdAsync(Guid id);
    }
}
