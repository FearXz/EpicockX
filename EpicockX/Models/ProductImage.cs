namespace EpicockX.Models
{
    public class ProductImage
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ProductImageUrl { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
