using EpicockX.Models;
using Microsoft.Data.SqlClient;

namespace EpicockX.Services
{
    public class OrderService
    {
        private readonly IConfiguration _config;

        public OrderService(IConfiguration config)
        {
            _config = config;
        }

        public List<Order> GetOrders()
        {
            try
            {
                List<Order> orders = new List<Order>();
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string SELECT_ALL_COMMAND = "SELECT * FROM Orders";
                    using (SqlCommand cmd = new SqlCommand(SELECT_ALL_COMMAND, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order order = new Order();
                                order.OrderId = reader.GetInt32(0);
                                order.Name = reader.GetString(1);
                                order.Surname = reader.GetString(2);
                                order.Address = reader.GetString(3);
                                order.City = reader.GetString(4);
                                order.ZipCode = reader.GetString(5);
                                order.Country = reader.GetString(6);
                            }
                        }
                    }
                }
                return orders;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero della lista degli ordini", ex);
            }
        }

        public Order GetOrderById(int id)
        {
            try
            {
                Order order = new Order();
                using (
                    SqlConnection conn = new SqlConnection(
                        _config.GetConnectionString("DefaultConnection")
                    )
                )
                {
                    conn.Open();
                    const string SELECT_BY_ID_COMMAND =
                        "SELECT * FROM Orders WHERE OrderId = @OrderId";
                    using (SqlCommand cmd = new SqlCommand(SELECT_BY_ID_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                order.OrderId = reader.GetInt32(0);
                                order.Name = reader.GetString(1);
                                order.Surname = reader.GetString(2);
                                order.Address = reader.GetString(3);
                                order.City = reader.GetString(4);
                                order.ZipCode = reader.GetString(5);
                                order.Country = reader.GetString(6);
                            }
                        }
                    }
                }
                return order;
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero dell'ordine", ex);
            }
        }

        public void AddOrder(Order order)
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
                        "INSERT INTO Orders (Name, Surname, Address, City, ZipCode, Country) VALUES (@Name, @Surname, @Address, @City, @ZipCode, @Country)";
                    using (SqlCommand cmd = new SqlCommand(INSERT_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", order.Name);
                        cmd.Parameters.AddWithValue("@Surname", order.Surname);
                        cmd.Parameters.AddWithValue("@Address", order.Address);
                        cmd.Parameters.AddWithValue("@City", order.City);
                        cmd.Parameters.AddWithValue("@ZipCode", order.ZipCode);
                        cmd.Parameters.AddWithValue("@Country", order.Country);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'inserimento dell'ordine", ex);
            }
        }

        public void UpdateOrder(Order order)
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
                        "UPDATE Orders SET Name = @Name, Surname = @Surname, Address = @Address, City = @City, ZipCode = @ZipCode, Country = @Country WHERE OrderId = @OrderId";
                    using (SqlCommand cmd = new SqlCommand(UPDATE_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", order.Name);
                        cmd.Parameters.AddWithValue("@Surname", order.Surname);
                        cmd.Parameters.AddWithValue("@Address", order.Address);
                        cmd.Parameters.AddWithValue("@City", order.City);
                        cmd.Parameters.AddWithValue("@ZipCode", order.ZipCode);
                        cmd.Parameters.AddWithValue("@Country", order.Country);
                        cmd.Parameters.AddWithValue("@OrderId", order.OrderId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'aggiornamento dell'ordine", ex);
            }
        }

        public void DeleteOrder(int id)
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
                    const string DELETE_COMMAND = "DELETE FROM Orders WHERE OrderId = @OrderId";
                    using (SqlCommand cmd = new SqlCommand(DELETE_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nell'eliminazione dell'ordine", ex);
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
