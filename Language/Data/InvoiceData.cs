using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class InvoiceData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public InvoiceData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<Invoice> GetAll()
        {
            List<Invoice> invoices = new List<Invoice>();

            string query = @"
                           SELECT
                        di.detail_invoice_id,
                        di.schedule_id,
                        i.invoice_id,
                        i.user_id,
                        i.invoice_number,
                        i.invoice_date,
                        c.course_name,
                        ca.category_name,
                        cs.course_date,
                        c.price AS course_price,
                        COUNT(*) AS total_course,
                        (
                            SELECT COUNT(*)
                            FROM course c
                            JOIN course_schedule cs ON c.course_id = cs.course_id
                            JOIN detail_invoice di ON cs.schedule_id = di.schedule_id
                            WHERE di.detail_invoice_id = detail_invoice_id
                        ) AS total_course,
                        (
                            SELECT SUM(c.price)
                            FROM course c
                            JOIN course_schedule cs ON c.course_id = cs.course_id
                            JOIN detail_invoice di ON cs.schedule_id = di.schedule_id
                            WHERE di.detail_invoice_id = detail_invoice_id
                        ) AS total_price
                    FROM
                        invoice i
                    JOIN
                        detail_invoice di ON i.invoice_id = di.invoice_id
                    JOIN
                        course_schedule cs ON di.schedule_id = cs.schedule_id
                    JOIN
                        course c ON cs.course_id = c.course_id
                    JOIN
                        category ca ON c.category_id = ca.category_id
                    GROUP BY
                        i.invoice_id, di.detail_invoice_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try { 
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        Invoice currentInvoice = null;

                        while (reader.Read())
                        {
                            if (currentInvoice == null || currentInvoice.invoice_id != Guid.Parse(reader["invoice_id"].ToString()))
                            {
                                currentInvoice = new Invoice
                                {
                                    invoice_id = Guid.Parse(reader["invoice_id"].ToString()),
                                    user_id = Guid.Parse(reader["user_id"].ToString()),
                                    invoice_number = int.Parse(reader["invoice_number"].ToString()),
                                    total_price = int.Parse(reader["total_price"].ToString()),
                                    invoice_date = Convert.ToDateTime(reader["invoice_date"]),
                                    detail_Invoices = new List<Detail_Invoice>()
                                };

                                invoices.Add(currentInvoice);
                            }

                            if (currentInvoice != null)
                            {
                                currentInvoice.detail_Invoices.Add(new Detail_Invoice
                                {
                                    detail_invoice_id = Guid.Parse(reader["detail_invoice_id"].ToString()),
                                    invoice_id = Guid.Parse(reader["invoice_id"].ToString()),
                                    schedule_id = Guid.Parse(reader["schedule_id"].ToString()),
                                    category_name = reader["category_name"].ToString(),
                                    course_name = reader["course_name"].ToString(),
                                    course_date = Convert.ToDateTime(reader["course_date"])
                                });
                            }
                        }
                    }
                }catch (Exception ex)
                    {
                        throw new Exception("Failed to get data", ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return invoices;
        }

        //Insert Invoice
        public bool InsertInvoice(Invoice invoice)
        {
            bool result = false;
            string query = "INSERT INTO invoice (invoice_id, user_id, invoice_number, invoice_date, total_price) " +
                   "VALUES (@invoice_id, @user_id, @invoice_number, @invoice_date, @total_price)";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@invoice_id", invoice.invoice_id);
                        command.Parameters.AddWithValue("@user_id", invoice.user_id);
                        command.Parameters.AddWithValue("@invoice_number", invoice.invoice_number);
                        command.Parameters.AddWithValue("@invoice_date", invoice.invoice_date);
                        command.Parameters.AddWithValue("@total_price", invoice.total_price);

                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            result = true;
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return result;
        }
    }
}

