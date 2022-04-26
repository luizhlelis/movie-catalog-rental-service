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
        TotalPrice = itemsTotalPrice;
        PaymentMethod = paymentMethod;
        Status = OrderStatus.Created;
        Customer = customer;
    }

    // Empty constructor required for EF
    public Order() { }

    public double TotalPrice { get; private set; }

    public double ShippingPrice { get; private set; }

    public double ItemsTotalPrice { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public OrderStatus Status { get; private set; }

    [JsonIgnore] public virtual User Customer { get; set; }

    // Stub Method
    public void LoadShippingPrice(string originZipCode)
    {
        // destiny ZipCode = Customer.ZipCode, Address = Customer.Adress
        ShippingPrice = 10;
    }

    public void LoadTotalPrice()
    {
        TotalPrice = ItemsTotalPrice + ShippingPrice;
    }

    public void FinalizeIt()
    {
        Status = OrderStatus.Finished;
    }

    public void DeleteIt()
    {
        Status = OrderStatus.Deleted;
    }
}
