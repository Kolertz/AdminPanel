using AdminPanel.Models.Dtos;

namespace AdminPanel.Interfaces;

public interface IRateService
{
    Task<RateDto?> GetCurrentRateAsync();
    Task<RateDto> UpdateRateAsync(RateDto newRate);
}
