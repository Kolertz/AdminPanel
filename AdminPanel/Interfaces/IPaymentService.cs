using AdminPanel.Models.Dtos;
using AdminPanel.Models.Entities;

namespace AdminPanel.Interfaces;

public interface IPaymentService
{
    Task<List<PaymentDto>> GetRecentPaymentsAsync(int take);
}
