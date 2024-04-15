using Language.DTOs.Checkout;
using Language.DTOs.DetailCheckout;
using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class CheckoutData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public CheckoutData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        //private readonly string connectionString = "server=localhost;port=3306;database=language;user=root;password=";

        //SelectAll
        public List<CheckoutDTO> GetAll()
        {
            List<CheckoutDTO> checkouts = new List<CheckoutDTO>();

            string query = @"
        SELECT
            co.checkout_id,
            co.user_id,
            co.id_payment_method,
            dc.detail_checkout_id,
            dc.course_id,
            dc.checklist,
            CONCAT(ca.category_name, ' - ', c.course_name) AS category_course,
            c.course_name,
            cs.course_date
        FROM
            checkout co
        JOIN
            detail_checkout dc ON co.checkout_id = dc.checkout_id
        JOIN
            course c ON dc.course_id = c.course_id
        JOIN
            category ca ON c.category_id = ca.category_id
        JOIN
            course_schedule cs ON c.course_id = cs.course_id
        ORDER BY
            co.checkout_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        CheckoutDTO currentCheckout = null;

                        while (reader.Read())
                        {
                            if (currentCheckout == null || currentCheckout.checkout_id != Guid.Parse(reader["checkout_id"].ToString()))
                            {
                                currentCheckout = new CheckoutDTO
                                {
                                    checkout_id = Guid.Parse(reader["checkout_id"].ToString()),
                                    user_id = Guid.Parse(reader["user_id"].ToString()),
                                    id_payment_method = Guid.Parse(reader["id_payment_method"].ToString()),
                                    checkout_detail = new List<DetailShowCheckoutDTO>()
                                };

                                checkouts.Add(currentCheckout);
                            }

                            if (currentCheckout != null)
                            {
                                currentCheckout.checkout_detail.Add(new DetailShowCheckoutDTO
                                {
                                    detail_checkout_id = Guid.Parse(reader["detail_checkout_id"].ToString()),
                                    course_id = Guid.Parse(reader["course_id"].ToString()),
                                    checklist = reader.GetBoolean(reader.GetOrdinal("checklist")),
                                    category_course = reader["category_course"].ToString(),
                                    course_name = reader["course_name"].ToString(),
                                    course_date = Convert.ToDateTime(reader["course_date"])
                                });
                            }
                        }
                    }
                }
            }

            return checkouts;
        }



        //SelectAllByCheckoutId
        public List<CheckoutDTO> GetAllByCheckoutId(Guid checkout_id)
        {
            List<CheckoutDTO> checkouts = new List<CheckoutDTO>();

            string query = @"
                SELECT
                    co.checkout_id,
                    co.user_id,
                    co.id_payment_method,
                    dc.detail_checkout_id,
                    dc.course_id,
                    dc.checklist,
                    CONCAT(ca.category_name, ' - ', c.course_name) AS category_course,
                    c.course_name,
                    cs.course_date
                FROM
                    checkout co
                JOIN
                    detail_checkout dc ON co.checkout_id = dc.checkout_id
                JOIN
                    course c ON dc.course_id = c.course_id
                JOIN
                    category ca ON c.category_id = ca.category_id
                JOIN
                    course_schedule cs ON c.course_id = cs.course_id
                WHERE
                    co.checkout_id = @checkout_id";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@checkout_id", checkout_id);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        CheckoutDTO currentCheckout = null;

                        while (reader.Read())
                        {
                            if (currentCheckout == null)
                            {
                                currentCheckout = new CheckoutDTO
                                {
                                    checkout_id = Guid.Parse(reader["checkout_id"].ToString()),
                                    user_id = Guid.Parse(reader["user_id"].ToString()),
                                    id_payment_method = Guid.Parse(reader["id_payment_method"].ToString()),
                                    checkout_detail = new List<DetailShowCheckoutDTO>()
                                };

                                checkouts.Add(currentCheckout);
                            }

                            currentCheckout.checkout_detail.Add(new DetailShowCheckoutDTO
                            {
                                detail_checkout_id = Guid.Parse(reader["detail_checkout_id"].ToString()),
                                course_id = Guid.Parse(reader["course_id"].ToString()),
                                checklist = reader.GetBoolean(reader.GetOrdinal("checklist")),
                                category_course = reader["category_course"].ToString(),
                                course_name = reader["course_name"].ToString(),
                                course_date = Convert.ToDateTime(reader["course_date"])
                            });
                        }
                    }
                }
            }

            return checkouts;
        }


        //Insert detail_checkout
        public bool InsertDetailCheckout(Detail_Checkout detailCheckout)
        {
            bool result = false;
            string query = $"INSERT INTO detail_checkout (detail_checkout_id, checkout_id, course_id, checklist) " +
                     $"VALUES (@detail_checkout_id, @checkout_id, @course_id, @checklist)";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using(MySqlCommand command = new MySqlCommand()) 
                {
                    command.Parameters.AddWithValue("@detail_checkout_id",detailCheckout.detail_checkout_id);
                    command.Parameters.AddWithValue("@checkout_id", detailCheckout.checkout_id);
                    command.Parameters.AddWithValue("@course_id", detailCheckout.course_id);
                    command.Parameters.AddWithValue("@checklist", detailCheckout.checklist);

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