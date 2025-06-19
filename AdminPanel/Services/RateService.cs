using AdminPanel.Interfaces;
using AdminPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class RateService(AppDbContext db) : IRateService
{
    private readonly AppDbContext _db = db;

    public async Task<Rate?> GetCurrentRateAsync() =>
        await _db.Rates.AsNoTracking().FirstOrDefaultAsync();

    public async Task<Rate> UpdateRateAsync(Rate newRate)
    {
        var rate = await _db.Rates.FirstOrDefaultAsync();
        if (rate == null)
        {
            _db.Rates.Add(newRate);
        }
        else
        {
            rate.Value = newRate.Value;
        }
        await _db.SaveChangesAsync();
        return newRate;
    }
}
