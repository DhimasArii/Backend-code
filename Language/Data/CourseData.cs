using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class CourseData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public CourseData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }


        //private readonly string connectionString = "server=localhost;port=3306;database=language;user=root;password=";

        //SelectAll
        public List<Category> GetAll()
        {
            List<Category> categories = new List<Category>();

            string query = @"
                SELECT
                    ca.category_id,
                    ca.category_name,
                    ca.category_description,
                    ca.category_image,
                    c.course_id,
                    c.course_name,
                    c.course_description,
                    c.course_image,
                    c.price
                FROM
                    category ca
                JOIN
                    course c ON ca.category_id = c.category_id
                ORDER BY
                    ca.category_id, c.course_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        Category currentCategory = null;

                        while (reader.Read())
                        {
                            if (currentCategory == null || currentCategory.category_id != Guid.Parse(reader["category_id"].ToString()))
                            {
                                currentCategory = new Category
                                {
                                    category_id = Guid.Parse(reader["category_id"].ToString()),
                                    category_name = reader["category_name"].ToString(),
                                    category_description = reader["category_description"].ToString(),
                                    category_image = reader["category_image"].ToString(),
                                    courses = new List<Course>()
                                };

                                categories.Add(currentCategory);
                            }

                            if (currentCategory != null)
                            {
                                currentCategory.courses.Add(new Course
                                {
                                    course_id = Guid.Parse(reader["course_id"].ToString()),
                                    category_id = Guid.Parse(reader["category_id"].ToString()),
                                    course_name = reader["course_name"].ToString(),
                                    course_description = reader["course_description"].ToString(),
                                    course_image = reader["course_image"].ToString(),
                                    price = int.Parse(reader["price"].ToString())
                                });
                            }
                        }
                    }
                }
            }

            return categories;
        }

        //SelectAllCategory
        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();

            string query = @"
                SELECT
                    category_id,
                    category_name,
                    category_description,
                    category_image
                FROM
                    category";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Category category = new Category
                            {
                                category_id = Guid.Parse(reader["category_id"].ToString()),
                                category_name = reader["category_name"].ToString(),
                                category_description = reader["category_description"].ToString(),
                                category_image = reader["category_image"].ToString()
                            };

                            categories.Add(category);
                        }
                    }
                }
            }

            return categories;
        }

        //SelectAllByCategoryId
        public List<Course> GetAllCoursesByCategoryId(Guid category_id)
        {
            List<Course> courses = new List<Course>();

            string query = @"
        SELECT
            course_id,
            category_id,
            course_name,
            course_description,
            course_image,
            price
        FROM
            course
        WHERE
            category_id = @category_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@category_id", category_id);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Course course = new Course
                            {
                                course_id = Guid.Parse(reader["course_id"].ToString()),
                                category_id = Guid.Parse(reader["category_id"].ToString()),
                                course_name = reader["course_name"].ToString(),
                                course_description = reader["course_description"].ToString(),
                                course_image = reader["course_image"].ToString(),
                                price = int.Parse(reader["price"].ToString())
                            };

                            courses.Add(course);
                        }
                    }
                }
            }

            return courses;
        }



        //Insert detail_checkout
        public bool InsertDetailCheckout(Detail_Checkout detailCheckout)
        {
            bool result = false;
            string query = $"INSERT INTO detail_checkout (detail_checkout_id, checkout_id, course_id, checklist) " +
                     $"VALUES (@detail_checkout_id, @checkout_id, @course_id, @checklist)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@detail_checkout_id", detailCheckout.detail_checkout_id);
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
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@checklist", detailCheckout.checklist);
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

