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
        dc.schedule_id,
        dc.checklist,
        ca.category_name,
        c.course_name,
        cs.course_date
    FROM
        checkout co
    JOIN
        detail_checkout dc ON co.checkout_id = dc.checkout_id
    JOIN
        course_schedule cs ON dc.schedule_id = cs.schedule_id
    JOIN
        course c ON cs.course_id = c.course_id
    JOIN
        category ca ON c.category_id = ca.category_id
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
                                    schedule_id = Guid.Parse(reader["schedule_id"].ToString()),
                                    checklist = reader.GetBoolean(reader.GetOrdinal("checklist")),
                                    category_name = reader["category_name"].ToString(),
                                    course_name = reader["course_name"].ToString(),
                                    course_date = Convert.ToDateTime(reader["course_date"])
                                });
                            }
                        }
                    }

                    connection.Close();
                }
            }

            return checkouts;
        }



        //SelectAllByUserId
        public List<Checkout> GetAllByUserId(Guid user_id,string sortOrder)
        {
            List<Checkout> checkouts = new List<Checkout>();

            string query = @"
    SELECT
        co.checkout_id,
        co.user_id,
        co.id_payment_method,
        co.create_date,
        dc.detail_checkout_id,
        dc.schedule_id,
        dc.checklist,
        ca.category_name,
        c.course_name,
        c.course_description,
        c.course_image,
        c.price,
        cs.course_date
    FROM
        checkout co
    JOIN
        detail_checkout dc ON co.checkout_id = dc.checkout_id
    JOIN
        course_schedule cs ON dc.schedule_id = cs.schedule_id
    JOIN
        course c ON cs.course_id = c.course_id
    JOIN
        category ca ON c.category_id = ca.category_id
    WHERE
        co.user_id = @user_id
    ORDER BY 
        co.create_date " + sortOrder + @", co.checkout_id";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", user_id);
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
                                    create_date = Convert.ToDateTime(reader["create_date"]),
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
                                    schedule_id = Guid.Parse(reader["schedule_id"].ToString()),
                                    checklist = reader.GetBoolean(reader.GetOrdinal("checklist")),
                                    category_name = reader["category_name"].ToString(),
                                    course_name = reader["course_name"].ToString(),
                                    course_description = reader["course_description"].ToString(),
                                    course_image = reader["course_image"].ToString(),
                                    price = int.Parse(reader["price"].ToString()),
                                    course_date = Convert.ToDateTime(reader["course_date"])
                                });
                            }
                        }
                    }
                }
                connection.Close();
            }

            return checkouts;
        }

        //Insert checkout
        public bool InsertCheckout(Checkout checkout)
        {
            bool result = false;
            string query = $"INSERT INTO checkout (checkout_id, user_id) " +
                     $"VALUES (@checkout_id, @user_id)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@checkout_id", checkout.checkout_id);
                    command.Parameters.AddWithValue("@user_id", checkout.user_id);
                    //command.Parameters.AddWithValue("@id_payment_method", checkout.id_payment_method);

                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }
            return result;

        }

        //Insert detail_checkout
        public bool InsertDetailCheckout(Detail_Checkout detailCheckout)
        {
            bool result = false;
            string query = $"INSERT INTO detail_checkout (detail_checkout_id, checkout_id, schedule_id, checklist) " +
                     $"VALUES (@detail_checkout_id, @checkout_id, @schedule_id, @checklist)";

            using(MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using(MySqlCommand command = new MySqlCommand()) 
                {
                    command.Parameters.AddWithValue("@detail_checkout_id",detailCheckout.detail_checkout_id);
                    command.Parameters.AddWithValue("@checkout_id", detailCheckout.checkout_id);
                    command.Parameters.AddWithValue("@schedule_id", detailCheckout.schedule_id);
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

        //Create for BuyNow
        public bool CreateBuyNow(Checkout checkout, Detail_Checkout detailCheckout)
        {
            bool result = false;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    MySqlCommand command1 = new MySqlCommand();
                    command1.Connection = connection;
                    command1.Transaction = transaction;
                    command1.Parameters.Clear();

                    command1.CommandText = "INSERT INTO checkout (checkout_id, user_id, id_payment_method, create_date) VALUES (@checkout_id, @user_id, @id_payment_method, @create_date)";
                    command1.Parameters.AddWithValue("@checkout_id", checkout.checkout_id);
                    command1.Parameters.AddWithValue("@user_id", checkout.user_id);
                    command1.Parameters.AddWithValue("@id_payment_method", checkout.id_payment_method);
                    command1.Parameters.AddWithValue("@create_date", checkout.create_date);

                    MySqlCommand command2 = new MySqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.Parameters.Clear();

                    command2.CommandText = "INSERT INTO detail_checkout (detail_checkout_id, checkout_id, schedule_id, checklist) VALUES (@detail_checkout_id, @checkout_id, @schedule_id, @checklist)";
                    command2.Parameters.AddWithValue("@detail_checkout_id", detailCheckout.detail_checkout_id);
                    command2.Parameters.AddWithValue("@checkout_id", detailCheckout.checkout_id);
                    command2.Parameters.AddWithValue("@schedule_id", detailCheckout.schedule_id);
                    command2.Parameters.AddWithValue("@checklist", detailCheckout.checklist);

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
                    transaction.Rollback();
                    throw new Exception("Failed to create buy now", ex);
                }
                finally
                {
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