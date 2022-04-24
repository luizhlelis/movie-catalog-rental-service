using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Ports;

namespace RentMovie.Infrastructure.Adapters;

public class PaymentAdapter : IPaymentDrivenPort
{
    // Stub Method
    public Task ChargeUser(double totalPrice, PaymentMethod paymentMethod, string username)
    {
        return Task.CompletedTask;
    }
}
