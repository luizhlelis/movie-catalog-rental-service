using Newtonsoft.Json;
using RentMovie.Application.Domain.Enums;

namespace RentMovie.Application.Domain.Entities;

public class Order : IdentifiableEntity
{
    public Order(
        User customer,
        double itemsTotalPrice,
        PaymentMethod paymentMethod)
    {
        ItemsTotalPrice = itemsTotalPrice;
        PaymentMethod = paymentMethod;
        Status = OrderStatus.Created;
        Customer = customer;
    }

    public double TotalPrice { get; private set; }

    public double ShippingPrice { get; private set; }

    public double ItemsTotalPrice { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public OrderStatus Status { get; private set; }

    [JsonIgnore] public virtual User Customer { get; set; }

    // Stub Method
    public void LoadShippingPrice(string originZipCode)
    {
        // destiny ZipCode = Customer.ZipCode
        ShippingPrice = 10;
    }

    public void LoadTotalPrice()
    {
        TotalPrice = ItemsTotalPrice + ShippingPrice;
    }
}
