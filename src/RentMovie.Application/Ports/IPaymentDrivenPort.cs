using RentMovie.Application.Domain.Enums;

namespace RentMovie.Application.Ports;

public interface IPaymentDrivenPort
{
    Task ChargeUser(double totalPrice, PaymentMethod paymentMethod, string username);
}
