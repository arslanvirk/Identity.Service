using Identity.Service.Application.DTOs;
using Identity.Service.Core.IRepositories;
using Identity.Service.Application.IServices;

namespace Identity.Service.Application.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly IConfigRepository _configRepo;
    public ConfigurationService(IConfigRepository configRepo) => _configRepo = configRepo;

    public async Task<ConfigurationDto> GetByKeyAsync(string key)
    {
        var cfg = await _configRepo.GetByKeyAsync(key);
        return new ConfigurationDto(cfg.ConfigurationId, cfg.ConfigurationKey, cfg.ConfigurationValue);
    }
}