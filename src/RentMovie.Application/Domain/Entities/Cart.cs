using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace RentMovie.Application.Domain.Entities;

/// <summary>
/// Stores temporarily the customer acquisition intention. This is a cache class, cart is not
/// mapped in Entity Framework.
/// </summary>
public class Cart
{
    public Cart()
    {
        TotalPrice = 0;
        Items = new Dictionary<Guid, CartItem>();
    }

    public Cart(byte[] bytes)
    {
        var bytesAsString = Encoding.UTF8.GetString(bytes);
        var deserializedCart = JsonConvert.DeserializeObject<Cart>(bytesAsString);
        TotalPrice = deserializedCart.TotalPrice;
        Items = deserializedCart.Items;
    }

    public double TotalPrice { get; private set; }

    public Dictionary<Guid, CartItem> Items { get; private set; }

    public void AddItem(CartItem item)
    {
        if (Items.ContainsKey(item.Id))
            Items[item.Id].IncreaseAmount();
        else
            Items.Add(item.Id, item);

        TotalPrice += item.Price;
    }

    public void AddItem(Movie item)
    {
        var cartItem = new CartItem(item.Id, item.Title, item.RentCategory.Price, 1);

        if (Items.ContainsKey(item.Id))
            Items[item.Id].IncreaseAmount();
        else
            Items.Add(item.Id, cartItem);

        TotalPrice += cartItem.Price;
    }

    public bool IsEmpty()
    {
        return !Items.Any();
    }

    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
    }
}

public class CartItem
{
    public CartItem(Guid id, string name, double price, int amount)
    {
        Id = id;
        Name = name;
        Price = price;
        Amount = amount;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public double Price { get; private set; }

    public int Amount { get; private set; }

    public void IncreaseAmount()
    {
        Amount++;
    }
}
