using System;
using Microsoft.EntityFrameworkCore;
using Identity.Service.Core.Entities;
using Identity.Service.Core.IRepositories;
using Identity.Service.Infrastructure.Context;

namespace Identity.Service.Infrastructure.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly EFDataContext _ctx;
        public ConfigRepository(EFDataContext ctx) => _ctx = ctx;

        public async Task<Configuration> GetByKeyAsync(string key)
            => await _ctx.Configurations.FirstOrDefaultAsync(c => c.ConfigurationKey != null && c.ConfigurationKey == key)
               ?? throw new KeyNotFoundException();

        public async Task<Configuration> GetByIdAsync(Guid id)
            => await _ctx.Set<Configuration>().FindAsync(id)
               ?? throw new KeyNotFoundException();
    }
}
