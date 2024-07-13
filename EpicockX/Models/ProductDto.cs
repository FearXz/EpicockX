using System.ComponentModel.DataAnnotations;

namespace EpicockX.Models
{
    public class ProductDto
    {
        [Required(ErrorMessage = "Il nome del prodotto è obbligatorio.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "La descrizione del prodotto è obbligatoria.")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "La quantità del prodotto è obbligatoria.")]
        [Range(
            0,
            int.MaxValue,
            ErrorMessage = "La quantità del prodotto deve essere un numero non negativo."
        )]
        public int ProductQuantity { get; set; }

        [Required(ErrorMessage = "Il prezzo del prodotto è obbligatorio.")]
        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "Il prezzo del prodotto deve essere un numero non negativo."
        )]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "La categoria del prodotto è obbligatoria.")]
        public string ProductCategory { get; set; }

        [Required(ErrorMessage = "La marca del prodotto è obbligatoria.")]
        public string ProductBrand { get; set; }

        [Required(ErrorMessage = "Inserire un url immagine prodotto")]
        public string ProductImageUrl { get; set; }
    }
}
