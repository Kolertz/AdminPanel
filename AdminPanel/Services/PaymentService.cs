using AdminPanel.Interfaces;
using AdminPanel.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Services;

public class PaymentService(AppDbContext db) : IPaymentService
{
    private readonly AppDbContext _db = db;

    public async Task<List<Payment>> GetRecentPaymentsAsync(int take) =>
        await _db.Payments
            .AsNoTracking()
            .Include(p => p.Client)
            .OrderByDescending(p => p.Date)
            .Take(take)
            .ToListAsync();
}
