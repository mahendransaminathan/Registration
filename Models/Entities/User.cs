using System.ComponentModel.DataAnnotations;
public class User
{
    [Key]
    public int Id { get; set; } 
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string UserType { get; set; }
}