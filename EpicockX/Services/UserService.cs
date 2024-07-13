using EpicockX.Models;
using Microsoft.Data.SqlClient;

namespace EpicockX.Services
{
    public class UserService
    {
        private readonly IConfiguration _config;

        public UserService(IConfiguration config)
        {
            _config = config;
        }

        public User GetUser(LoginDto loginDto)
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
                        "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                    using (SqlCommand cmd = new SqlCommand(SELECT_BY_ID_COMMAND, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", loginDto.Username);
                        cmd.Parameters.AddWithValue("@Password", loginDto.Password);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                User user = new User
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    Password = reader.GetString(2)
                                };
                                return user;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Errore nel recupero dell'utente", ex);
            }
            return null;
        }
    }
}
