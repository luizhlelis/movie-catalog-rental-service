using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentMovie.Application.Domain.ValueObjects;

namespace RentMovie.Application.Domain.Entities;

public class User
{
    public User(string username, Password password)
    {
        Username = username;
        Password = password;
        PasswordHash = password.Hash;
    }

    // Empty constructor required for EF
    public User() { }

    [Key] [Required] [MaxLength(20)] public string Username { get; private set; }

    [Required] public string PasswordHash { get; private set; }

    [NotMapped] public Password Password { get; }
}
