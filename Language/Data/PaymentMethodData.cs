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

    }
}
