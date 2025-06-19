using AdminPanel.Models;

namespace AdminPanel.Interfaces;

public interface IRateService
{
    Task<Rate?> GetCurrentRateAsync();
    Task<Rate> UpdateRateAsync(Rate newRate);
}
