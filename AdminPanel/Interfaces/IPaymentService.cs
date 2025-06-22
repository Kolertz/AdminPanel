using AdminPanel.Models.Dtos;

namespace AdminPanel.Interfaces;

public interface IPaymentService
{
    Task<List<PaymentDto>> GetRecentPaymentsAsync(int take);
}
