using EpicockX.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace EpicockX.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _http;
        private readonly IConfiguration _config;

        public CartService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _http = httpContextAccessor;
            _config = config;
        }

        public List<Product> GetCart()
        {
            List<Product> products = new List<Product>();
            if (_http.HttpContext.Session.GetString("Cart") != null)
            {
                string cartJson = _http.HttpContext.Session.GetString("Cart");
                products = JsonConvert.DeserializeObject<List<Product>>(cartJson);
            }
            return products;
        }

        public void SaveCart(List<Product> cart)
        {
            string cartJson = JsonConvert.SerializeObject(cart);
            _http.HttpContext.Session.SetString("Cart", cartJson);
        }

        public void ClearCart()
        {
            _http.HttpContext.Session.Remove("Cart");
        }

        public int SubmitOrder(Order order, int userId, List<Product> products)
        {
            try
            {
                int OrderId;
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            const string INSERT_COMMAND =
                                "INSERT INTO Orders (Name, Surname, Address, City, ZipCode, Country, UserId)"
                                + " VALUES (@Name, @Surname, @Address, @City, @ZipCode, @Country, @UserId)"
                                + " SELECT SCOPE_IDENTITY()";
                            using (
                                SqlCommand cmd = new SqlCommand(INSERT_COMMAND, conn, transaction)
                            )
                            {
                                cmd.Parameters.AddWithValue("@Name", order.Name);
                                cmd.Parameters.AddWithValue("@Surname", order.Surname);
                                cmd.Parameters.AddWithValue("@Address", order.Address);
                                cmd.Parameters.AddWithValue("@City", order.City);
                                cmd.Parameters.AddWithValue("@ZipCode", order.ZipCode);
                                cmd.Parameters.AddWithValue("@Country", order.Country);
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                OrderId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            foreach (Product product in products)
                            {
                                const string INSERT_PRODUCT_COMMAND =
                                    "INSERT INTO OrderProducts (OrderId, ProductId, Quantity) VALUES (@OrderId, @ProductId, 1)";
                                using (
                                    SqlCommand cmd = new SqlCommand(
                                        INSERT_PRODUCT_COMMAND,
                                        conn,
                                        transaction
                                    )
                                )
                                {
                                    cmd.Parameters.AddWithValue("@OrderId", OrderId);
                                    cmd.Parameters.AddWithValue("@ProductId", product.ProductId);
                                    cmd.Parameters.AddWithValue("@Quantity", 1);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Errore nell'inserimento dell'ordine", ex);
                        }
                    }
                }
                return OrderId;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'aggiunta del prodotto", ex);
            }
        }
    }
}
