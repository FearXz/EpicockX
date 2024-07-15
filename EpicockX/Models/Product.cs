using System.ComponentModel.DataAnnotations;

namespace EpicockX.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        [Display(Name = "Nome Prodotto")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Descrizione")]
        public string ProductDescription { get; set; }
        [Required]
        [Display(Name = "Quantità")]
        public int ProductQuantity { get; set; } = 1;
        [Required]
        [Display(Name = "Prezzo")]
        public decimal ProductPrice { get; set; }
        [Required]
        [Display(Name = "Categoria")]
        public string ProductCategory { get; set; }
        [Required]
        [Display(Name = "Marca")]
        public string ProductBrand { get; set; }
        public string? ProductImage { get; set; }
        public List<string> ProductImages { get; set; } = new List<string>();

        public decimal TotalPrice => ProductPrice * ProductQuantity;
    }
}
