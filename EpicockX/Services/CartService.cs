using System.Data;
using EpicockX.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Stripe.Checkout;

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

        public Session CreateCheckoutSession(List<Product> cart)
        {
            var domain = "https://localhost:7231/";

            var Options = new SessionCreateOptions
            {
                SuccessUrl = domain + "Cart/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = domain + "Cart/Fail",
                PaymentMethodTypes = new List<string> { "card", "paypal" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in cart)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.ProductPrice * 100,
                        Currency = "eur",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName,
                            Description = item.ProductDescription,
                        }
                    },
                    Quantity = 1
                };
                Options.LineItems.Add(sessionLineItem);
            }
            // creo la sessione di Stripe e la invio al client
            var service = new SessionService();
            Session session = service.Create(Options);

            return session;
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
                            transaction.Dispose();
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

        public ResultOrderDto GetResultOrder(string sessionId)
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
                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (
                                SqlCommand cmd = new SqlCommand("setPagamento", conn, transaction)
                            )
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@SessionId", sessionId);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        resultOrder.OrderId = reader.GetInt32(0);
                                        resultOrder.ProductList = reader.GetString(1);
                                    }
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
                return resultOrder;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'aggiunta del prodotto", ex);
            }
        }
    }
}
