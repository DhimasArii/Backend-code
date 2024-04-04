using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class UserData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public UserData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        //private readonly string connectionString = "server=localhost;port=3306;database=language;user=root;password=";

        //SelectAll
        public List<User> GetAll()
        {
            List<User> books = new List<User>();
            string query = "SELECT * FROM user";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new User
                            {
                                user_id = Guid.Parse(reader["user_id"].ToString() ?? string.Empty),
                                email = reader["email"].ToString() ?? string.Empty,
                                password = reader["password"].ToString() ?? string.Empty,
                                address = reader["address"].ToString() ?? string.Empty,
                                phone_number = reader["phone_number"].ToString() ?? string.Empty,
                            });
                        }
                    }

                    connection.Close();

                }

            }

            return books;

        }

        //Select By Primary Key
        public User? GetById(Guid id)
        {
            User? user = null;

            string query = $"SELECT * FROM user WHERE user_id = @id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();


                    using (MySqlDataReader reader = command.ExecuteReader())

                    {
                        while (reader.Read())
                        {
                            user = new User
                            {
                                user_id = Guid.Parse(reader["user_id"].ToString() ?? string.Empty),
                                email = reader["email"].ToString() ?? string.Empty,
                                password = reader["password"].ToString() ?? string.Empty,
                                address = reader["address"].ToString() ?? string.Empty,
                                phone_number = reader["phone_number"].ToString() ?? string.Empty,
                            };
                        }
                    }
                }

                connection.Close();
            }

            return user;
        }

        //Insert
        public bool Insert(User user)
        {
            bool result = false;


            string query = $"INSERT INTO user(user_id, email, password, address, phone_number) " + $"VALUES (@user_id, @email, @password, @address, @phone_number)";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@user_id", user.user_id);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@password", user.password);
                    command.Parameters.AddWithValue("@address", user.address);
                    command.Parameters.AddWithValue("@phone_number", user.phone_number);

                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;

        }

        //Update
        public bool Update(Guid id, User user)
        {
            bool result = false;


            string query = $"UPDATE user SET email = @email, password = @password, address = @address, phone_number = @phone_number WHERE user_id = @user_id";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@user_id", id);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@password", user.password);
                    command.Parameters.AddWithValue("@address", user.address);
                    command.Parameters.AddWithValue("@phone_number", user.phone_number);

                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;
        }

        // Delete
        public bool Delete(Guid id)
        {
            bool result = false;

            string query = $"DELETE FROM user WHERE user_id = @user_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@user_id", id);

                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;
        }
    }
}