using AdminPanel.Models;

namespace AdminPanel.Interfaces;

public interface IPaymentService
{
    Task<List<Payment>> GetRecentPaymentsAsync(int take);
}
