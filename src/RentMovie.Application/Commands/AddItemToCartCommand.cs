namespace RentMovie.Application.Commands;

public class AddItemToCartCommand
{
    public Guid MovieId { get; set; }

    public string Username { get; set; }
}
