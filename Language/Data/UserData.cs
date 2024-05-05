using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class UserData
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public UserData(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        //private readonly string connectionString = "server=localhost;port=3306;database=language;user=root;password=";

        //SelectAll
        public List<User> GetAll()
        {
            List<User> users = new List<User>();
            string query = "SELECT u.user_id, u.email, u.passwords,u.IsActivated, r.role " +
                           "FROM users u " +
                           "JOIN user_role r ON u.user_id = r.user_id";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                users.Add(new User
                                {
                                    user_id = Guid.Parse(reader["user_id"].ToString() ?? string.Empty),
                                    email = reader["email"].ToString() ?? string.Empty,
                                    passwords = reader["passwords"].ToString() ?? string.Empty,
                                    role = reader["role"].ToString() ?? string.Empty,
                                    IsActivated = Convert.ToBoolean(reader["IsActivated"])
                                });
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return users;
        }


        //Select By Primary Key
        public User? GetById(Guid id)
        {
            User? user = null;


            string query = $"SELECT * FROM users WHERE user_id = @id";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
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

                                passwords = reader["passwords"].ToString() ?? string.Empty,
                                
                            };
                        }
                    }
                }

                connection.Close();
            }

            return user;
        }

        //Insert
        //multiple sql command (with transaction)

        public bool CreateUserAccount(User user, UserRole userRole, Checkout checkout)
        {
            bool result = false;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    MySqlCommand command1 = new MySqlCommand();
                    command1.Connection = connection;
                    command1.Transaction = transaction;
                    command1.Parameters.Clear();

                    command1.CommandText = "INSERT INTO users (user_id, email, passwords, IsActivated) VALUES (@user_id, @email, @passwords, @isActivated)";
                    command1.Parameters.AddWithValue("@user_id", user.user_id);
                    command1.Parameters.AddWithValue("@email", user.email);
                    command1.Parameters.AddWithValue("@passwords", user.passwords);
                    command1.Parameters.AddWithValue("@isActivated", user.IsActivated);

                    MySqlCommand command2 = new MySqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.Parameters.Clear();

                    command2.CommandText = "INSERT INTO user_role (user_id, role) VALUES (@userId, @role)";
                    command2.Parameters.AddWithValue("@userId", userRole.user_id);
                    command2.Parameters.AddWithValue("@role", userRole.role);

                    MySqlCommand command3 = new MySqlCommand();
                    command3.Connection = connection;
                    command3.Transaction = transaction;
                    command3.Parameters.Clear();

                    command3.CommandText = "INSERT INTO checkout (checkout_id, user_id, create_date) VALUES (@checkout_id, @user_id, @create_date)";
                    command3.Parameters.AddWithValue("@checkout_id", Guid.NewGuid().ToString()); // Generate checkout_id
                    command3.Parameters.AddWithValue("@user_id", user.user_id);
                    command3.Parameters.AddWithValue("@create_date", DateTime.Now);

                    var result1 = command1.ExecuteNonQuery();
                    var result2 = command2.ExecuteNonQuery();
                    var result3 = command3.ExecuteNonQuery();

                    if (result1 > 0 && result2 > 0 && result3 > 0)
                    {
                        transaction.Commit();
                        result = true;
                    }
                    else
                    {
                        transaction.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Failed to create user account", ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }



        public User? CheckUserAuth(string email)
        {
            User? user = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * from users WHERE email = @email";

                    command.Parameters.Clear();

                    command.Parameters.AddWithValue("@email", email);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new User
                            {
                                user_id = Guid.Parse(reader["user_id"].ToString() ?? string.Empty),
                                email = reader["email"].ToString() ?? string.Empty,
                                passwords = reader["passwords"].ToString() ?? string.Empty,
                                IsActivated = Convert.ToBoolean(reader["IsActivated"])
                            };
                        }
                    }

                    connection.Close();
                }
            }

            return user;
        }

        public UserRole? GetUserRole(Guid user_id)
        {
            UserRole? userRole = null;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();

                    command.CommandText = "SELECT * from user_role WHERE user_id = @user_id";
                    command.Parameters.AddWithValue("@user_id", user_id);

                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userRole = new UserRole
                            {
                                id_user_role = Convert.ToInt32(reader["id_user_role"]),
                                user_id = Guid.Parse(reader["user_id"].ToString() ?? string.Empty),
                                role = reader["role"].ToString() ?? string.Empty
                            };
                        }
                    }

                    connection.Close();
                }
            }

            return userRole;
        }

        public bool ActivateUser(Guid id)
        {
            bool result = false;

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.Parameters.Clear();

                command.CommandText = "UPDATE Users SET IsActivated = 1 WHERE user_id = @user_id";
                command.Parameters.AddWithValue("@user_id", id);

                connection.Open();
                result = command.ExecuteNonQuery() > 0 ? true : false;

                connection.Close();
            }

            return result;
        }

        public bool ResetPassword(string email, string password)
        {
            bool result = false;

            string query = "UPDATE Users SET passwords = @Password WHERE email = @Email";

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.Parameters.Clear();

                    command.CommandText = query;

                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);

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



            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

            using (MySqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    // Update data di tabel users
                    MySqlCommand command1 = connection.CreateCommand();
                    command1.Transaction = transaction;
                        // Cek apakah ada permintaan untuk mengubah password
                        if (!string.IsNullOrEmpty(user.passwords))
                        {
                            // Jika ada permintaan untuk mengubah password, tambahkan pernyataan UPDATE untuk password
                            command1.CommandText = @"
                        UPDATE users 
                        SET email = @email, 
                            passwords = @passwords, 
                            IsActivated = @isActivated 
                        WHERE user_id = @user_id";
                            command1.Parameters.AddWithValue("@passwords", user.passwords);
                        }
                        else
                        {
                            // Jika tidak ada permintaan untuk mengubah password, abaikan pernyataan UPDATE untuk password
                            command1.CommandText = @"
                        UPDATE users 
                        SET email = @email,
                            IsActivated = @isActivated 
                        WHERE user_id = @user_id";
                        }

                        // Tetap tambahkan parameter yang diperlukan
                        command1.Parameters.AddWithValue("@user_id", id);
                        command1.Parameters.AddWithValue("@email", user.email);
                        command1.Parameters.AddWithValue("@isActivated", user.IsActivated);


                        // Update data di tabel user_role
                        MySqlCommand command2 = connection.CreateCommand();
                    command2.Transaction = transaction;
                    command2.CommandText = @"
                    UPDATE user_role 
                    SET role = @role 
                    WHERE user_id = @user_id";
                    command2.Parameters.AddWithValue("@user_id", id);
                    command2.Parameters.AddWithValue("@role", user.role);

                        var result1 = command1.ExecuteNonQuery();
                        var result2 = command2.ExecuteNonQuery();

                        if (result1 > 0 && result2 > 0)
                        {
                            transaction.Commit();
                            result = true;
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                catch (Exception ex)
                {
                    // Rollback transaksi jika terjadi kesalahan
                    transaction.Rollback();
                    Console.WriteLine("Error updating user and user_role:", ex.Message);
                }
            }

            connection.Close();
        }

    return result;
}

    // Delete
    public bool Delete(Guid id)
        {
            bool result = false;

            string query = $"DELETE FROM users WHERE user_id = @user_id";


            using (MySqlConnection connection = new MySqlConnection(_connectionString))
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