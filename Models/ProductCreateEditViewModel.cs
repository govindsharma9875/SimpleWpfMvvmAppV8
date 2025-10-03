using System.ComponentModel.DataAnnotations;

namespace SimpleWpfToMvcApp.Web.Models;

public class ProductCreateEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    [Display(Name = "Product Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be between $0.01 and $999,999.99")]
    [Display(Name = "Price")]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "SKU is required")]
    [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters")]
    [Display(Name = "SKU")]
    public string SKU { get; set; } = string.Empty;

    [Display(Name = "Product Image")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Current Image")]
    public string? ImagePath { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    public bool IsEdit => Id > 0;
    public string PageTitle => IsEdit ? "Edit Product" : "Add New Product";
    public string ButtonText => IsEdit ? "Update Product" : "Create Product";
}
