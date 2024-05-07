using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class PaymentMethodData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public PaymentMethodData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //Select all payment method
        public List<Payment> GetPayments()
        {
            List<Payment> payments = new List<Payment>();

            string query = $"SELECT * FROM payment_method ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Payment payment = new Payment
                            {
                                id_payment_method = Guid.Parse(reader["id_payment_method"].ToString()),
                                payment_name = reader["payment_name"].ToString(),
                                payment_description = reader["payment_description"].ToString(),
                                payment_icon = reader["payment_icon"].ToString(),
                                payment_status = reader.GetBoolean(reader.GetOrdinal("payment_status")),
                            };
                            payments.Add(payment);
                        }
                    }

                }
            }
            return payments;
        }

        //Insert Payment method
        public bool CreatePaymentMethod(Payment payment)
        {
            bool result = false;
            string query = @"INSERT INTO payment_method (id_payment_method, payment_name, payment_description, payment_icon, payment_status) 
                                     VALUES (@id_payment_method, @payment_name, @payment_description, @payment_icon, @payment_status)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id_payment_method", payment.id_payment_method);
                    command.Parameters.AddWithValue("@payment_name", payment.payment_name);
                    command.Parameters.AddWithValue("@payment_description", payment.payment_description);
                    command.Parameters.AddWithValue("@payment_icon", payment.payment_icon);
                    command.Parameters.AddWithValue("@payment_status", payment.payment_status);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0;

                    connection.Close();
                }
            }

            return result;
        }

        // Update Payment method
        public bool UpdatePaymentMethod(Guid id_payment_method, Payment payment)
        {
            bool result = false;
            string query = @"UPDATE payment_method 
                     SET payment_name = @payment_name, 
                         payment_description = @payment_description, 
                         payment_icon = @payment_icon, 
                         payment_status = @payment_status 
                     WHERE id_payment_method = @id_payment_method";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@payment_name", payment.payment_name);
                    command.Parameters.AddWithValue("@payment_description", payment.payment_description);
                    command.Parameters.AddWithValue("@payment_icon", payment.payment_icon);
                    command.Parameters.AddWithValue("@payment_status", payment.payment_status);
                    command.Parameters.AddWithValue("@id_payment_method", id_payment_method);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0;

                    connection.Close();
                }
            }

            return result;
        }

        // Delete
        public bool Delete(Guid id_payment_method)
        {
            bool result = false;

            string query = $"DELETE FROM payment_method WHERE id_payment_method = @id_payment_method";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@id_payment_method", id_payment_method);

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
