using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using RentMovie.Application.Domain.Enums;
using RentMovie.Application.Domain.ValueObjects;

namespace RentMovie.Application.Domain.Entities;

public class User
{
    public User(string username, string password, Role role = Role.Customer)
    {
        Username = username;
        Password = new Password(password);
        PasswordHash = Password.Hash;
        Role = role;
    }

    // Empty constructor required for EF
    public User() { }

    [Key] [Required] [MaxLength(20)] public string Username { get; private set; }

    [Required] [JsonIgnore] public string PasswordHash { get; private set; }

    [Required] [JsonIgnore] public Role Role { get; private set; }

    [NotMapped] [JsonIgnore] private readonly Password Password;
}
