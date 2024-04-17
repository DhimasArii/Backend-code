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
        LEFT JOIN
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
                                if (reader["course_id"] != DBNull.Value)
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



        //Insert category
        public bool CreateCategory(Category category)
        {
            bool result = false;
            string query = $"INSERT INTO Category (category_id, category_name, category_description, category_image) " +
                     $"VALUES (@category_id, @category_name, @category_description, @category_image)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@category_id", category.category_id);
                    command.Parameters.AddWithValue("@category_name", category.category_name);
                    command.Parameters.AddWithValue("@category_description", category.category_description);
                    command.Parameters.AddWithValue("@category_image", category.category_image);


                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }
            return result;

        }

        //Update category
        public bool UpdateCategory(Guid category_id, Category category)
        {
            bool result = false;
            string query = $"UPDATE Category SET category_name = @category_name, " +
                           $"category_description = @category_description, category_image = @category_image " +
                           $"WHERE category_id = @category_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@category_name", category.category_name);
                    command.Parameters.AddWithValue("@category_description", category.category_description);
                    command.Parameters.AddWithValue("@category_image", category.category_image);
                    command.Parameters.AddWithValue("@category_id", category_id);

                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }
            return result;
        }

        //Insert course
        public bool CreateCourse(Course course)
        {
            bool result = false;
            string query = $"INSERT INTO course (course_id, category_id, course_name, course_description, course_image, price) " +
                           $"VALUES (@course_id, @category_id, @course_name, @course_description, @course_image, @price)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@course_id", course.course_id);
                    command.Parameters.AddWithValue("@category_id", course.category_id);
                    command.Parameters.AddWithValue("@course_name", course.course_name);
                    command.Parameters.AddWithValue("@course_description", course.course_description);
                    command.Parameters.AddWithValue("@course_image", course.course_image);
                    command.Parameters.AddWithValue("@price", course.price);

                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }
            return result;
        }

        //Update course
        public bool UpdateCourse(Guid course_id, Course course)
        {
            bool result = false;
            string query = $"UPDATE course SET category_id = @category_id, course_name = @course_name, " +
                           $"course_description = @course_description, course_image = @course_image, price = @price " +
                           $"WHERE course_id = @course_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@category_id", course.category_id);
                    command.Parameters.AddWithValue("@course_name", course.course_name);
                    command.Parameters.AddWithValue("@course_description", course.course_description);
                    command.Parameters.AddWithValue("@course_image", course.course_image);
                    command.Parameters.AddWithValue("@price", course.price);
                    command.Parameters.AddWithValue("@course_id", course_id);

                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }
            return result;
        }

        //Delete course
        public bool DeleteCourse(Guid course_id)
        {
            bool result = false;
            string query = $"DELETE FROM course WHERE course_id = @course_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@course_id", course_id);

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

