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
            string query = "SELECT * FROM users";
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try { 
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
                                    
                                });
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally { 
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
        public bool CreateUserAccount(User user, UserRole userRole)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

<<<<<<< HEAD
                    command1.CommandText = "INSERT INTO users (user_id, email, passwords, IsActivated) VALUES (@user_id, @email, @passwords, @isActivated)";
                    command1.Parameters.AddWithValue("@user_id", user.user_id);
                    command1.Parameters.AddWithValue("@email", user.email);
                    command1.Parameters.AddWithValue("@passwords", user.passwords);
                    command1.Parameters.AddWithValue("@isActivated", user.IsActivated);
=======
                    // Begin transaction
                    using (MySqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Insert user data
                            string insertUserQuery = "INSERT INTO users (user_id, email, passwords) VALUES (@user_id, @email, @password)";
                            MySqlCommand insertUserCommand = new MySqlCommand(insertUserQuery, connection, transaction);
                            insertUserCommand.Parameters.AddWithValue("@user_id", user.user_id);
                            insertUserCommand.Parameters.AddWithValue("@email", user.email);
                            insertUserCommand.Parameters.AddWithValue("@password", user.passwords);
                            insertUserCommand.ExecuteNonQuery();
>>>>>>> refs/remotes/origin/master_new

                            // Insert user role data
                            string insertUserRoleQuery = "INSERT INTO user_role (user_id, role) VALUES (@user_id, @role)";
                            MySqlCommand insertUserRoleCommand = new MySqlCommand(insertUserRoleQuery, connection, transaction);
                            insertUserRoleCommand.Parameters.AddWithValue("@user_id", userRole.user_id);
                            insertUserRoleCommand.Parameters.AddWithValue("@role", userRole.role);
                            insertUserRoleCommand.ExecuteNonQuery();

                            // Commit transaction
                            transaction.Commit();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction on error
                            transaction.Rollback();
                            throw new Exception("Failed to create user account.", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to connect to the database.", ex);
            }
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



            string query = $"UPDATE users SET email = @email, passwords = @passwords, address = @address, phone_number = @phone_number WHERE user_id = @user_id";



            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@user_id", id);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@passwords", user.passwords);
                    

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