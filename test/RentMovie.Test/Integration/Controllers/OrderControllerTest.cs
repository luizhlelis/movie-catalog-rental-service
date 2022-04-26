using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RentMovie.Application.Domain.Entities;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Domain.ValueObjects;
using RentMovie.Application.Dtos;
using RentMovie.Application.Ports;
using RentMovie.Test.Helpers;
using Xunit;

namespace RentMovie.Test.Integration.Controllers;

public class OrderControllerTest : IntegrationTestFixture
{
    private const string OrderPath = "v1/order";
    private readonly IDistributedCache _cache;
    private readonly Authentication _auth;
    private readonly IDatabaseDrivenPort _databaseDrivenPort;

    public OrderControllerTest()
    {
        _auth = TestServiceScope.ServiceProvider.GetRequiredService<Authentication>();
        _cache = TestServiceScope.ServiceProvider.GetRequiredService<IDistributedCache>();
        _databaseDrivenPort =
            TestServiceScope.ServiceProvider.GetRequiredService<IDatabaseDrivenPort>();
    }

    [Fact(DisplayName = "Should return ok when order exists")]
    public async Task GetOrder_WhenOrderExists_ShouldReturnOk()
    {
        // arrange
        var username = "order-controller-user-0";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        var order = new Order(user, 10, PaymentMethod.PayPal);
        await DbContext.Users.AddAsync(user);
        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.GetAsync($"{OrderPath}/?orderId={order.Id}");

        // assert
        response.Should().Be200Ok().And.BeAs(new
        {
            totalPrice = order.TotalPrice,
            shippingPrice = order.ShippingPrice,
            itemsTotalPrice = order.ItemsTotalPrice,
            paymentMethod = order.PaymentMethod,
            status = order.Status,
            customer = new
            {
                username = order.Customer.Username,
                zipCode = order.Customer.ZipCode,
                address = order.Customer.Address,
                givenName = order.Customer.GivenName
            },
            id = order.Id.ToString()
        });
    }

    [Fact(DisplayName = "Should return not found when order does not exist")]
    public async Task GetOrder_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        // arrange
        var username = "order-controller-user-1";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        var order = new Order(user, 10, PaymentMethod.PayPal);
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.GetAsync($"{OrderPath}/?orderId={order.Id}");

        // assert
        response.Should().Be404NotFound().And.BeAs(
            new
            {
                type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                title = "NOT_FOUND_ERROR",
                status = 404,
                traceId = "0HMH5DLVSLJDP",
                error = new
                {
                    msg = "There is no order registered with that Id"
                }
            },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName = "Should return not found when order exists for another user")]
    public async Task GetOrder_WhenOrderExistsForAnotherUser_ShouldReturnNotFound()
    {
        // arrange
        var username1 = "order-controller-user-3";
        var username2 = "order-controller-user-4";
        var users = new List<User>
        {
            new(username1, "StrongPass@123", "12345",
                "1458 Sauer Courts Suite 328", "John Doe"),
            new(username2, "StrongPass@123", "12345",
                "1458 Sauer Courts Suite 328", "John Doe")
        };
        var order = new Order(users.Last(), 10, PaymentMethod.PayPal);
        await DbContext.Users.AddRangeAsync(users);
        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(users.First().Username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.GetAsync($"{OrderPath}/?orderId={order.Id}");

        // assert
        response.Should().Be404NotFound().And.BeAs(
            new
            {
                type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                title = "NOT_FOUND_ERROR",
                status = 404,
                traceId = "0HMH5DLVSLJDP",
                error = new
                {
                    msg = "There is no order registered with that Id"
                }
            },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName = "Should return created on post order when cart exists")]
    public async Task PostOrder_WhenCartExists_ShouldReturnCreated()
    {
        // arrange
        var username = "order-controller-user-5";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var movie = new Movie("Titanic", "Boat in sea", 1994, 5);
        movie.RentCategory = new RentCategory("FakeOne", 10);
        var cart = new Cart();
        cart.AddItem(movie);
        await _cache.SetAsync(username, cart.ToBytes());
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var orderDto = new CreateOrderDto {PaymentMethod = PaymentMethod.PayPal};

        var content = new StringContent(
            JsonConvert.SerializeObject(orderDto),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(OrderPath, content);

        // assert
        response.Should().Be201Created().And.BeAs(new
        {
            totalPrice = cart.TotalPrice + 10,
            shippingPrice = 10,
            itemsTotalPrice = cart.TotalPrice,
            paymentMethod = orderDto.PaymentMethod,
            status = OrderStatus.Created,
            customer = new
            {
                username = user.Username,
                zipCode = user.ZipCode,
                address = user.Address,
                givenName = user.GivenName
            },
            id = ""
        }, options => options.Excluding(source => source.id));
    }

    [Fact(DisplayName = "Should return not found on post order when cart does not exist")]
    public async Task PostOrder_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        // arrange
        var username = "order-controller-user-6";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var movie = new Movie("Titanic", "Boat in sea", 1994, 5);
        movie.RentCategory = new RentCategory("FakeOne", 10);

        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var orderDto = new CreateOrderDto {PaymentMethod = PaymentMethod.PayPal};

        var content = new StringContent(
            JsonConvert.SerializeObject(orderDto),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(OrderPath, content);

        // assert
        response.Should().Be404NotFound().And.BeAs(new
            {
                type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                title = "NOT_FOUND_ERROR",
                status = 404,
                traceId = "",
                error = new
                {
                    msg = "Cart not found or expired, you must create it first"
                }
            },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName = "Should return bad request on post order when cart is empty")]
    public async Task PostOrder_WhenCartIsEmpty_ShouldReturnBadRequest()
    {
        // arrange
        var username = "order-controller-user-7";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        var cart = new Cart();
        await _cache.SetAsync(username, cart.ToBytes());
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var orderDto = new CreateOrderDto {PaymentMethod = PaymentMethod.PayPal};

        var content = new StringContent(
            JsonConvert.SerializeObject(orderDto),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PostAsync(OrderPath, content);

        // assert
        response.Should().Be400BadRequest().And.BeAs(new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                title = "BAD_REQUEST_ERROR",
                status = 400,
                traceId = "",
                error = new
                {
                    msg = "Your cart is empty!"
                }
            },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName = "Should return no content on finish when order exists and cart is valid")]
    public async Task PuFinishOrder_WhenOrderExistsAndValidCart_ShouldReturnNoContent()
    {
        // arrange
        var username = "order-controller-user-8";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        var movie = new Movie("Titanic 2", "Boat in sea returns", 1995, 10);
        movie.RentCategory = new RentCategory("Nice Movies", 10);
        var cart = new Cart();
        cart.AddItem(movie);
        cart.AddItem(movie);
        cart.AddItem(movie);
        cart.AddItem(movie);
        cart.AddItem(movie);
        await _cache.SetAsync(username, cart.ToBytes());
        var order = new Order(user, cart.TotalPrice, PaymentMethod.PayPal);
        await DbContext.Movies.AddAsync(movie);
        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var orderDto = new OrderDto {OrderId = order.Id};

        var content = new StringContent(
            JsonConvert.SerializeObject(orderDto),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PutAsync($"{OrderPath}/finish", content);

        // assert
        response.Should().Be204NoContent();
        var cachedCart = await _cache.GetAsync(username);
        cachedCart.Should().BeNull();
    }

    [Fact(DisplayName = "Should return not found on finish when order exists and cart is null")]
    public async Task PuFinishOrder_WhenCartIsNull_ShouldReturnNotFound()
    {
        // arrange
        var username = "order-controller-user-11";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        await DbContext.Users.AddAsync(user);
        var movie = new Movie("Titanic 2", "Boat in sea returns", 1995, 10);
        movie.RentCategory = new RentCategory("FakeOne", 10);
        var order = new Order(user, 60, PaymentMethod.PayPal);
        await DbContext.Movies.AddAsync(movie);
        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        var orderDto = new OrderDto {OrderId = order.Id};

        var content = new StringContent(
            JsonConvert.SerializeObject(orderDto),
            Encoding.UTF8,
            "application/json");

        // act
        var response = await Client.PutAsync($"{OrderPath}/finish", content);

        // assert
        response.Should().Be404NotFound().And.BeAs(
            new
            {
                type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                title = "NOT_FOUND_ERROR",
                status = 404,
                traceId = "",
                error = new
                {
                    msg = "Cart not found or expired, you must create it first"
                }
            },
            options => options.Excluding(source => source.traceId));
    }

    [Fact(DisplayName = "Should delete order when it exists and status is created")]
    public async Task DeleteOrder_WhenOrderExists_ShouldReturnOk()
    {
        // arrange
        var username = "order-controller-user-9";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        var order = new Order(user, 10, PaymentMethod.PayPal);
        await DbContext.Users.AddAsync(user);
        await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();
        var movie = new Movie("Titanic 2", "Boat in sea returns", 1995, 10);
        movie.RentCategory = new RentCategory("FakeOne", 10);
        var cart = new Cart();
        cart.AddItem(movie);
        await _cache.SetAsync(username, cart.ToBytes());
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.DeleteAsync($"{OrderPath}/?orderId={order.Id}");

        // assert
        response.Should().Be200Ok().And.BeAs(new
        {
            totalPrice = order.TotalPrice,
            shippingPrice = order.ShippingPrice,
            itemsTotalPrice = order.ItemsTotalPrice,
            paymentMethod = order.PaymentMethod,
            status = OrderStatus.Deleted,
            customer = new
            {
                username = order.Customer.Username,
                zipCode = order.Customer.ZipCode,
                address = order.Customer.Address,
                givenName = order.Customer.GivenName
            },
            id = order.Id.ToString()
        });
    }

    [Fact(DisplayName = "Should return not found on delete order when order does not exist")]
    public async Task DeleteOrder_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        // arrange
        var username = "order-controller-user-10";
        var user = new User(username, "StrongPass@123", "12345",
            "1458 Sauer Courts Suite 328", "John Doe");
        var order = new Order(user, 10, PaymentMethod.PayPal);
        await DbContext.Users.AddAsync(user);
        // await DbContext.Orders.AddAsync(order);
        await DbContext.SaveChangesAsync();
        var movie = new Movie("Titanic 2", "Boat in sea returns", 1995, 10);
        movie.RentCategory = new RentCategory("FakeOne", 10);
        var cart = new Cart();
        cart.AddItem(movie);
        await _cache.SetAsync(username, cart.ToBytes());
        var accessToken = _auth.GenerateAccessToken(username);
        Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        // act
        var response = await Client.DeleteAsync($"{OrderPath}/?orderId={order.Id}");

        // assert
        response.Should().Be404NotFound().And.BeAs(
            new
            {
                type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                title = "NOT_FOUND_ERROR",
                status = 404,
                traceId = "",
                error = new
                {
                    msg = "There is no order registered with that Id"
                }
            },
            options => options.Excluding(source => source.traceId));
    }
}
