using Identity.Service.Application.DTOs;

namespace Identity.Service.Application.IServices
{
    public interface IConfigurationService
    {
        Task<ConfigurationDto> GetByKeyAsync(string key);
    }
}
