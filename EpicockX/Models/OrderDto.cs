using System.ComponentModel.DataAnnotations;

namespace EpicockX.Models
{
    public class OrderGetDto
    {
        [Required(ErrorMessage = "Il nome è obbligatorio.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "L'indirizzo è obbligatorio.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "La città è obbligatoria.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Il CAP è obbligatorio.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Il paese è obbligatorio.")]
        public string Country { get; set; }

        public int UserId { get; set; }
    }

    public class OrderPostDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}
