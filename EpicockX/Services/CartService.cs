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
                                "INSERT INTO Orders (Name, Surname, Address, City, ZipCode, Country, UserId, SessionId)"
                                + " VALUES (@Name, @Surname, @Address, @City, @ZipCode, @Country, @UserId, @SessionId)"
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
                                cmd.Parameters.AddWithValue("@SessionId", order.SessionId);
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

        public ResultOrderDto GetResultOrder(int orderId, int userId)
        {
            ResultOrderDto resultOrder = new ResultOrderDto();
            try
            {
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string SELECT_ALL_COMMAND =
                        "SELECT\nOrders.OrderId AS \"Numero Ordine\",\nSTRING_AGG(CONCAT(OrderProducts.Quantity, 'x ', Products.ProductName, ''), ', ') AS \"Prodotti\"\nFROM\nOrders\nJOIN\nOrderProducts ON Orders.OrderId = OrderProducts.OrderId\nJOIN\nProducts ON OrderProducts.ProductId = Products.ProductId\nWHERE\nOrders.UserId = @userId\nGROUP BY\nOrders.OrderID";
                    using (SqlCommand cmd = new SqlCommand(SELECT_ALL_COMMAND, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CartProduct order = new CartProduct();
                                order.ProductName = reader.GetString(0);
                                order.ProductDescription = reader.GetString(1);
                                order.Quantity = reader.GetInt32(2);
                                order.ProductPrice = reader.GetInt32(3);
                                order.ProductCategory = reader.GetString(4);
                                order.ProductBrand = reader.GetString(5);
                            }
                        }
                    }
                }
                return resultOrder;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero della lista degli ordini", ex);
            }
        }
    }
}
