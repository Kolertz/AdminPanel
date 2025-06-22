using AdminPanel.Models.Entities;

namespace AdminPanel.Interfaces;

public interface IPaymentService
{
    Task<List<Payment>> GetRecentPaymentsAsync(int take);
}
