using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    [MetadataType(typeof(FoodCategoryMetadata))]
    public partial class FoodCategory
    {
    }

    public class FoodCategoryMetadata
    {
        [Required(ErrorMessage = "Naziv kategorije je obavezan")]
        [StringLength(128, ErrorMessage = "Naziv ne smije biti duži od 128 znakova")]
        [Display(Name = "Naziv kategorije")]
        public string Name { get; set; }
    }
}