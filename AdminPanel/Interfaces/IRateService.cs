using AdminPanel.Models.Entities;

namespace AdminPanel.Interfaces;

public interface IRateService
{
    Task<Rate?> GetCurrentRateAsync();
    Task<Rate> UpdateRateAsync(Rate newRate);
}
