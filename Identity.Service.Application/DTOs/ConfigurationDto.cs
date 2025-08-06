namespace Identity.Service.Application.DTOs;

public record ConfigurationDto(
    Guid ConfigurationId,
    string Key,
    string Value
);
