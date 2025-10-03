using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleWpfToMvcApp.Web.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    [Display(Name = "Product Name")]
    public required string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
    [Display(Name = "Price")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    [Display(Name = "SKU")]
    public required string SKU { get; set; }

    [StringLength(500, ErrorMessage = "Image path cannot exceed 500 characters")]
    [Display(Name = "Image Path")]
    public string? ImagePath { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
