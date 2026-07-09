using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models;

public class ProductViewModel
{
    [Required(ErrorMessage ="Product name is required.")]
    [StringLength(100, MinimumLength =5, ErrorMessage ="Product name should contain 5 and 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage ="Product SKU Code is required.")]
    [StringLength(20, MinimumLength =3, ErrorMessage ="Product SKU Code should contain 3 to 20 characters.")]
    public string SkuCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product description is required.")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "Product description should contain 10 to 200 characters.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product price is required.")]
    [Range(0.1, 9999, ErrorMessage = "Product price must be greater than 0.")]
    public decimal UnitPrice { get; set; }

    [Required(ErrorMessage ="Product category must be assigned.")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Image Url is required.")]
    [Url(ErrorMessage ="Enter a valid url.")]
    public string? ImageUrl { get; set; }
}