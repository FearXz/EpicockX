namespace EpicockX.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductCategory { get; set; }
        public string ProductBrand { get; set; }
        public string ProductImage { get; set; }
        public List<string> ProductImages { get; set; } = new List<string>();
    }
}
