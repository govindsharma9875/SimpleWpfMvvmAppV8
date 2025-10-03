using System.ComponentModel.DataAnnotations;

namespace SimpleWpfToMvcApp.Web.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    [Display(Name = "Email Address")]
    public required string Email { get; set; }
}
