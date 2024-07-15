
namespace EpicockX.Models
{
    public class BackOfficeIndexViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public Product NewProduct { get; set; } = new Product();
        public List<ProductImage> ProductImages { get; set; }
    }
}
