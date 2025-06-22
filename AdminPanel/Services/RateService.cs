using AdminPanel.Interfaces;
using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class RateService(AppDbContext db) : IRateService
{
    private readonly AppDbContext _db = db;

    public async Task<RateDto?> GetCurrentRateAsync() =>
        (await _db.Rates.AsNoTracking().FirstOrDefaultAsync())
        .Adapt<RateDto>();

    public async Task<RateDto> UpdateRateAsync(RateDto newRate)
    {
        var rate = await _db.Rates.FirstOrDefaultAsync();
        if (rate == null)
        {
            _db.Rates.Add(newRate.Adapt<Rate>());
        }
        else
        {
            rate.Value = newRate.Value;
        }
        await _db.SaveChangesAsync();
        return newRate;
    }
}
