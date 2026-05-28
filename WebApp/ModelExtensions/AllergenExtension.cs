using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    [MetadataType(typeof(AllergenMetadata))]
    public partial class Allergen
    {
    }

    public class AllergenMetadata
    {
        [Required(ErrorMessage = "Naziv alergena je obavezan")]
        [StringLength(128, ErrorMessage = "Naziv ne smije biti duži od 128 znakova")]
        [Display(Name = "Naziv alergena")]
        public string Name { get; set; }

        [StringLength(512, ErrorMessage = "Opis ne smije biti duži od 512 znakova")]
        [Display(Name = "Opis")]
        public string? Description { get; set; }
    }
}