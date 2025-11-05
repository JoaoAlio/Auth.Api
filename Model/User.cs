using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auth.Api.Model;

public class User
{
    [Key]
    public int Id { get; set; }
    [Column(TypeName = "varchar(100)")]
    public string? Name { get; set; } 
    [Required]
    [EmailAddress]
    [Column(TypeName = "varchar(150)")]
    public string? Email { get; set; }
    [Required] 
    [Column(TypeName = "varchar(255)")]
    public string? Password { get; set; } 
    public Guid UserIdentifier { get; set; }

}
