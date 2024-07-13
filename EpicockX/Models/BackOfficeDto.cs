using EpicockX.Models;

namespace EpicockX.ViewModels
{
    public class BackOfficeIndexViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public Product NewProduct { get; set; } = new Product();
        public Dictionary<int, List<ProductImage>> ProductImages { get; set; }
    }
}
