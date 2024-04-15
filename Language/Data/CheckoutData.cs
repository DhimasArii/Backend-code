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
        public List<Checkout> GetAll()
        {
            List<Checkout> checkouts = new List<Checkout>();

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
                        Checkout currentCheckout = null;

                        while (reader.Read())
                        {
                            if (currentCheckout == null || currentCheckout.checkout_id != Guid.Parse(reader["checkout_id"].ToString()))
                            {
                                currentCheckout = new Checkout
                                {
                                    checkout_id = Guid.Parse(reader["checkout_id"].ToString()),
                                    user_id = Guid.Parse(reader["user_id"].ToString()),
                                    id_payment_method = Guid.Parse(reader["id_payment_method"].ToString()),
                                    checkout_detail = new List<Detail_Checkout>()
                                };

                                checkouts.Add(currentCheckout);
                            }

                            if (currentCheckout != null)
                            {
                                currentCheckout.checkout_detail.Add(new Detail_Checkout
                                {
                                    detail_checkout_id = Guid.Parse(reader["detail_checkout_id"].ToString()),
                                    checkout_id = Guid.Parse(reader["checkout_id"].ToString()),
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
        public List<Checkout> GetAllByCheckoutId(Guid checkout_id)
        {
            List<Checkout> checkouts = new List<Checkout>();

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
                        Checkout currentCheckout = null;

                        while (reader.Read())
                        {
                            if (currentCheckout == null)
                            {
                                currentCheckout = new Checkout
                                {
                                    checkout_id = Guid.Parse(reader["checkout_id"].ToString()),
                                    user_id = Guid.Parse(reader["user_id"].ToString()),
                                    id_payment_method = Guid.Parse(reader["id_payment_method"].ToString()),
                                    checkout_detail = new List<Detail_Checkout>()
                                };

                                checkouts.Add(currentCheckout);
                            }

                            currentCheckout.checkout_detail.Add(new Detail_Checkout
                            {
                                detail_checkout_id = Guid.Parse(reader["detail_checkout_id"].ToString()),
                                checkout_id = Guid.Parse(reader["checkout_id"].ToString()),
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

        //Update detail_checkout
        public bool UpdateDetailCheckout(Guid detail_checkout_id, Detail_Checkout detailCheckout)
        {
            bool result = false;
            string query = $"UPDATE detail_checkout " +
                $"SET checklist = @checklist " +
                $"WHERE detail_checkout_id = @detail_checkout_id";
            
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using( MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@checklist", detailCheckout.checklist);
                    command.Parameters.AddWithValue("@detail_checkout_id", detail_checkout_id);

                    command.Connection = connection ;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }
            return result ;
        }

        //Delete detail_checkout
        public bool DeleteDetailCheckout(Guid detail_checkout_id)
        {
            bool result = false;
            string query = $"DELETE FROM detail_checkout WHERE detail_checkout_id = @detail_checkout_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@detail_checkout_id", detail_checkout_id);

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