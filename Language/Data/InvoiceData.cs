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
                        i.total_price,  
                        c.course_name,
                        ca.category_name,
                        cs.course_date,
                        c.price AS course_price,
                       (
                            SELECT COUNT(*)
                            FROM course c2
                            JOIN course_schedule cs2 ON c2.course_id = cs2.course_id
                            JOIN detail_invoice di2 ON cs2.schedule_id = di2.schedule_id
                            WHERE di2.invoice_id = i.invoice_id
                        ) AS total_course
                      
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
                        i.invoice_id, di.detail_invoice_id
                    ORDER BY
                         i.invoice_number asc, i.invoice_id";

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
                                    invoice_number = reader["invoice_number"].ToString(),
                                    total_price = int.Parse(reader["total_price"].ToString()),
                                    total_course = int.Parse(reader["total_course"].ToString()),
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
                                    course_price = int.Parse(reader["course_price"].ToString()),
                                    course_date = Convert.ToDateTime(reader["course_date"])
                                });
                            }
                        }
                    }
                }catch (Exception ex)
                    {
                        throw new Exception("Failed to get data" + ex.Message, ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return invoices;
        }

        //GetAllByInvoiceId
        public List<Invoice> GetAllByInvoiceId(Guid invoice_id)
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
                        i.total_price,  
                        c.course_name,
                        c.price AS course_price,
                        ca.category_name,
                        cs.course_date,
                       (
                            SELECT COUNT(*)
                            FROM course c2
                            JOIN course_schedule cs2 ON c2.course_id = cs2.course_id
                            JOIN detail_invoice di2 ON cs2.schedule_id = di2.schedule_id
                            WHERE di2.invoice_id = i.invoice_id
                        ) AS total_course
                      
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
                    WHERE
                        i.invoice_id = @invoice_id
                    GROUP BY
                        i.invoice_id, di.detail_invoice_id
                    ORDER BY
                         i.invoice_number asc, i.invoice_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@invoice_id", invoice_id);
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
                                        invoice_number = reader["invoice_number"].ToString(),
                                        total_price = int.Parse(reader["total_price"].ToString()),
                                        total_course = int.Parse(reader["total_course"].ToString()),
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
                                        course_price = int.Parse(reader["course_price"].ToString()),
                                        course_date = Convert.ToDateTime(reader["course_date"])
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to get data" + ex.Message, ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }

            return invoices;
        }


        //Create Invoice
        public bool CreateInvoice(Invoice invoice, Detail_Invoice detail_Invoice, Guid checkout_id)
        {
            bool result = false;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string invoiceNumber = GenerateInvoiceNumber();

                    // Set the generated invoice number to the invoice object
                    invoice.invoice_number = invoiceNumber;

                    //create invoice
                    MySqlCommand command1 = new MySqlCommand();
                    command1.Connection = connection;
                    command1.Transaction = transaction;
                    command1.Parameters.Clear();

                    command1.CommandText =  "INSERT INTO invoice (invoice_id, user_id, invoice_number,id_payment_method, invoice_date, total_price) " +
                                            "VALUES (@invoice_id, @user_id, @invoice_number,@id_payment_method, @invoice_date, @total_price)";
                    command1.Parameters.AddWithValue("@invoice_id", invoice.invoice_id);
                    command1.Parameters.AddWithValue("@user_id", invoice.user_id);
                    command1.Parameters.AddWithValue("@invoice_number", invoice.invoice_number);
                    command1.Parameters.AddWithValue("@id_payment_method", invoice.id_payment_method);
                    command1.Parameters.AddWithValue("@invoice_date", invoice.invoice_date);
                    command1.Parameters.AddWithValue("@total_price", invoice.total_price);

                    //add detail_invoice from detail_checkout 
                    MySqlCommand command2 = new MySqlCommand();
                    command2.Connection = connection;
                    command2.Transaction = transaction;
                    command2.Parameters.Clear();

                    command2.CommandText = @"INSERT INTO detail_invoice (detail_invoice_id, invoice_id, schedule_id)
                                            SELECT 
                                                UUID(),
                                                @invoice_id,
                                                dc.schedule_id
                                            FROM 
                                                detail_checkout dc
                                            JOIN
                                                checkout co ON dc.checkout_id = co.checkout_id
                                            WHERE 
                                                dc.checklist = TRUE
                                                AND co.user_id = @user_id
                                                AND dc.checkout_id = @checkout_id";

                    //command2.Parameters.AddWithValue("@detail_invoice_id", detail_Invoice.detail_invoice_id);
                    command2.Parameters.AddWithValue("@invoice_id", invoice.invoice_id);
                    command2.Parameters.AddWithValue("@user_id", invoice.user_id);
                    command2.Parameters.AddWithValue("@checkout_id", checkout_id);

                    //delete detail_checkout yang sudah jadi invoice
                    MySqlCommand command3 = new MySqlCommand();
                    command3.Connection = connection;
                    command3.Transaction = transaction;
                    command3.Parameters.Clear();

                    command3.CommandText = @"DELETE FROM detail_checkout
                         WHERE checklist = TRUE
                         AND checkout_id = @checkout_id";

                    command3.Parameters.AddWithValue("@checkout_id", checkout_id);

                    //update total_price
                    MySqlCommand command4 = new MySqlCommand();
                    command4.Connection = connection;
                    command4.Transaction = transaction;
                    command4.Parameters.Clear();

                    command4.CommandText = @"UPDATE invoice
                                         SET total_price = (SELECT SUM(c.price)
                                                            FROM course c
                                                            JOIN course_schedule cs ON c.course_id = cs.course_id
                                                            JOIN detail_invoice di ON cs.schedule_id = di.schedule_id
                                                            WHERE di.invoice_id = @invoice_id)
                                         WHERE invoice_id = @invoice_id";

                    command4.Parameters.AddWithValue("@invoice_id", invoice.invoice_id);





                    var result1 = command1.ExecuteNonQuery();
                    var result2 = command2.ExecuteNonQuery();
                    var result3 = command3.ExecuteNonQuery();
                    var result4 = command4.ExecuteNonQuery();

                    if (result1 > 0 && result2 > 0 && result3 > 0 && result4 > 0)
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
                    throw new Exception("Failed to create invoice", ex);
                }
                finally
                {
                    connection.Close();
                }
            }

            return result;
        }

        private string GenerateInvoiceNumber()
        {
            string prefix = "DLA";
            int startingNumber = 1;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;
                command.CommandText = "SELECT MAX(invoice_number) FROM invoice WHERE invoice_number LIKE 'DLA%'";

                var maxInvoiceNumber = command.ExecuteScalar();
                if (maxInvoiceNumber != null && maxInvoiceNumber != DBNull.Value)
                {
                    string lastInvoiceNumber = maxInvoiceNumber.ToString();
                    int lastNumber;
                    if (int.TryParse(lastInvoiceNumber.Substring(prefix.Length), out lastNumber))
                    {
                        startingNumber = lastNumber + 1;
                    }
                }
                else
                {
                    // Jika tidak ada invoice yang ada, set invoice_number ke DLA00001
                    return "DLA00001";
                }
            }

            string formattedNumber = startingNumber.ToString().PadLeft(5, '0');
            string invoiceNumber = prefix + formattedNumber;

            return invoiceNumber;
        }


    }
}

