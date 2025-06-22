using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;

namespace AdminPanel.Interfaces;

public interface IRateService
{
    Task<RateDto?> GetCurrentRateAsync();
    Task<RateDto> UpdateRateAsync(RateDto newRate);
}
