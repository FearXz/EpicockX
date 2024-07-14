using EpicockX.Models;
using Microsoft.Data.SqlClient;

namespace EpicockX.Services
{
    public class ProductService
    {
        private readonly IConfiguration _config;
        private readonly ImageService _imageSvc;

        public ProductService(IConfiguration config, ImageService imageService)
        {
            _config = config;
            _imageSvc = imageService;
        }

        public List<Product> GetProducts()
        {
            try
            {
                List<Product> products = new List<Product>();
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string SELECT_ALL_COMMAND =
                        "SELECT p.ProductId, p.ProductName, p.ProductDescription, p.ProductQuantity, p.ProductPrice, p.ProductCategory, p.ProductBrand, COALESCE(STRING_AGG(pi.ProductImageUrl, '?'),'') AS ProductImages FROM Products AS p LEFT JOIN ProductImages AS pi ON p.ProductId = pi.ProductId GROUP BY p.ProductId, p.ProductName, p.ProductDescription, p.ProductQuantity, p.ProductPrice, p.ProductCategory, p.ProductBrand";

                    using (SqlCommand cmd = new SqlCommand(SELECT_ALL_COMMAND, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product();
                                product.ProductId = reader.GetInt32(0);
                                product.ProductName = reader.GetString(1);
                                product.ProductDescription = reader.GetString(2);
                                product.ProductQuantity = reader.GetInt32(3);
                                product.ProductPrice = reader.GetDecimal(4);
                                product.ProductCategory = reader.GetString(5);
                                product.ProductBrand = reader.GetString(6);
                                product.ProductImage = reader.GetString(7);
                                product.ProductImages =
                                     String.IsNullOrEmpty(product.ProductImage) == false
                                        ? product.ProductImage.Split('?').ToList()
                                        : new List<string>();
                                products.Add(product);
                            }
                        }
                    }
                }
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero della lista dei prodotti", ex);
            }
        }

        public Product GetProductById(int id)
        {
            try
            {
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string SELECT_BY_ID_COMMAND =
                        "SELECT * FROM Products WHERE ProductId = @ProductId";
                    using (SqlCommand cmd = new SqlCommand(SELECT_BY_ID_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Product product = new Product();
                                product.ProductId = reader.GetInt32(0);
                                product.ProductName = reader.GetString(1);
                                product.ProductDescription = reader.GetString(2);
                                product.ProductQuantity = reader.GetInt32(3);
                                product.ProductPrice = reader.GetDecimal(4);
                                product.ProductCategory = reader.GetString(5);
                                product.ProductBrand = reader.GetString(6);
                                return product;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero del prodotto", ex);
            }
        }

        public void AddProduct(Product product)
        {
            try
            {
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string INSERT_COMMAND =
                        "INSERT INTO Products (ProductName, ProductDescription, ProductQuantity, ProductPrice, ProductCategory, ProductBrand) VALUES (@ProductName, @ProductDescription, @ProductQuantity, @ProductPrice, @ProductCategory, @ProductBrand)";
                    using (SqlCommand cmd = new SqlCommand(INSERT_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue(
                            "@ProductDescription",
                            product.ProductDescription
                        );
                        cmd.Parameters.AddWithValue("@ProductQuantity", product.ProductQuantity);
                        cmd.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                        cmd.Parameters.AddWithValue("@ProductCategory", product.ProductCategory);
                        cmd.Parameters.AddWithValue("@ProductBrand", product.ProductBrand);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'aggiunta del prodotto", ex);
            }
        }

        public void UpdateProduct(Product product)
        {
            try
            {
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string UPDATE_COMMAND =
                        "UPDATE Products SET ProductName = @ProductName, ProductDescription = @ProductDescription, ProductQuantity = @ProductQuantity, ProductPrice = @ProductPrice, ProductCategory = @ProductCategory, ProductBrand = @ProductBrand WHERE ProductId = @ProductId";
                    using (SqlCommand cmd = new SqlCommand(UPDATE_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                        cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
                        cmd.Parameters.AddWithValue(
                            "@ProductDescription",
                            product.ProductDescription
                        );
                        cmd.Parameters.AddWithValue("@ProductQuantity", product.ProductQuantity);
                        cmd.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                        cmd.Parameters.AddWithValue("@ProductCategory", product.ProductCategory);
                        cmd.Parameters.AddWithValue("@ProductBrand", product.ProductBrand);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'aggiornamento del prodotto", ex);
            }
        }

        public void DeleteProduct(int id)
        {
            try
            {
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string DELETE_COMMAND =
                        "DELETE FROM Products WHERE ProductId = @ProductId";
                    using (SqlCommand cmd = new SqlCommand(DELETE_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'eliminazione del prodotto", ex);
            }
        }

        public List<ProductImage> GetProductImages(int productId)
        {
            return _imageSvc.GetImages().Where(img => img.ProductId == productId).ToList();
        }

        public void AddProductImage(ProductImage image)
        {
            _imageSvc.AddImage(image);
        }

        public void DeleteProductImage(int id)
        {
            _imageSvc.DeleteImage(id);
        }
    }
}
