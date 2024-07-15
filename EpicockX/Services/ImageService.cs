using EpicockX.Models;
using Microsoft.Build.Globbing;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

namespace EpicockX.Services
{
    public class ImageService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public ImageService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public List<ProductImage> GetImages()
        {
            try
            {
                List<ProductImage> images = new List<ProductImage>();
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string SELECT_ALL_COMMAND = "SELECT * FROM ProductImages";
                    using (SqlCommand cmd = new SqlCommand(SELECT_ALL_COMMAND, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProductImage image = new ProductImage
                                {
                                    ProductImageId = reader.GetInt32(0),
                                    ProductId = reader.GetInt32(1),
                                    ProductImageUrl = reader.GetString(2)
                                };
                                images.Add(image);
                            }
                        }
                    }
                }
                return images;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Errore nel recupero della lista delle immagini dei prodotti",
                    ex
                );
            }
        }

        public ProductImage GetImageById(int id)
        {
            try
            {
                ProductImage image = null;
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string SELECT_BY_ID_COMMAND =
                        "SELECT * FROM ProductImages WHERE ProductImageId = @ProductImageId";
                    using (SqlCommand cmd = new SqlCommand(SELECT_BY_ID_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductImageId", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                image = new ProductImage
                                {
                                    ProductImageId = reader.GetInt32(0),
                                    ProductId = reader.GetInt32(1),
                                    ProductImageUrl = reader.GetString(2)
                                };
                            }
                        }
                    }
                }
                return image;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero dell'immagine del prodotto", ex);
            }
        }

        public void AddImage(ProductImage image)
        {
            try
            {
                if (image.ImageFile != null && image.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "assets/Products"); // prende la radice del progetto e concatena la cartella assets/Products
                    string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(image.ImageFile.FileName)}"; // crea un nome univoco per il file
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName); // concatena i due nomi precedenti per creare il percorso del file
                    using (var fileStream = new FileStream(filePath, FileMode.Create)) // crea un file stream (oggetto proprio di .net per salvare i file)
                    {
                        image.ImageFile.CopyTo(fileStream); // prende il file caricato e lo copia nel file stream
                    }
                    image.ProductImageUrl = $"/assets/Products/{uniqueFileName}"; // crei la path del file da salvare nel db
                }

                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string INSERT_COMMAND =
                        "INSERT INTO ProductImages (ProductId, ProductImageUrl) VALUES (@ProductId, @ProductImageUrl)";
                    using (SqlCommand cmd = new SqlCommand(INSERT_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", image.ProductId);
                        cmd.Parameters.AddWithValue("@ProductImageUrl", image.ProductImageUrl);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'aggiunta dell'immagine del prodotto", ex);
            }
        }

        public void UpdateImage(int productId, List<IFormFile> newImages)
        {
            try
            {
                // Rimuovi tutte le immagini esistenti per questo prodotto
                var existingImages = GetImages().Where(img => img.ProductId == productId).ToList();
                foreach (var image in existingImages)
                {
                    DeleteImage(image.ProductImageId);
                }

                // Aggiungi le nuove immagini
                foreach (var imageFile in newImages)
                {
                    if (imageFile.Length > 0)
                    {
                        // Crea un nuovo oggetto ProductImage per ogni immagine
                        var productImage = new ProductImage
                        {
                            ProductId = productId,
                            ImageFile = imageFile
                        };
                        AddImage(productImage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Errore durante l'aggiornamento delle immagini del prodotto.", ex);
            }
        }


        public void DeleteImage(int id)
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
                        "DELETE FROM ProductImages WHERE ProductImageId = @ProductImageId";
                    using (SqlCommand cmd = new SqlCommand(DELETE_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductImageId", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'eliminazione dell'immagine del prodotto", ex);
            }
        }
    }
}
