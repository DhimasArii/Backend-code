using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class UserData
    {
        private readonly string connectionString = "server=localhost;port=3306;database=language;user=root;password=";

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
                                user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
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
        public User? GetById(int id)
        {
            User? user = null;

            string query = $"SELECT * FROM user WHERE user_id = '{id}'";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {

                    connection.Open();


                    using (MySqlDataReader reader = command.ExecuteReader())

                    {
                        while (reader.Read())
                        {
                            user = new User
                            {
                                user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
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


            string query = $"INSERT INTO user(email, password, address, phone_number) " + $"VALUES ('{user.email}', '{user.password}', '{user.address}', '{user.phone_number}')";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
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
        public bool Update(int id, User user)
        {
            bool result = false;


            string query = $"UPDATE user SET email = '{user.email}', password = '{user.password}', address = '{user.address}', phone_number = '{user.phone_number}' WHERE user_id = '{id}'";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
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
        public bool Delete(int id)
        {
            bool result = false;

            string query = $"DELETE FROM user WHERE user_id = '{id}'";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
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