using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    [MetadataType(typeof(FoodMetadata))]
    [ValidateNever]
    public partial class Food
    {
    }

    public class FoodMetadata
    {
        [Required(ErrorMessage = "Naziv jela je obavezan")]
        [StringLength(256, ErrorMessage = "Naziv ne smije biti duži od 256 znakova")]
        [Display(Name = "Naziv")]
        public string Title { get; set; }

        [StringLength(1024, ErrorMessage = "Opis ne smije biti duži od 1024 znakova")]
        [Display(Name = "Opis")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Cijena je obavezna")]
        [Range(0.01, 10000, ErrorMessage = "Cijena mora biti između 0.01 i 10000")]
        [Display(Name = "Cijena")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "URL slike nije ispravan")]
        [Display(Name = "URL slike")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Dostupno")]
        public bool IsAvailable { get; set; }

        [Required(ErrorMessage = "Kategorija je obavezna")]
        [Display(Name = "Kategorija")]
        public int FoodCategoryId { get; set; }
    }
}