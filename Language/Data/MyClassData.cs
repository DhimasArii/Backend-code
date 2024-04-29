using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class MyClassData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        private readonly object myclass;

        public MyClassData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // SelectAll
        public List<My_Class> GetAll()
        {
            List<My_Class> myclass = new List<My_Class>();

            string query = @"
        SELECT
            mc.class_id,
            mc.user_id,
            mc.course_id,
            c.category_id,
            c.course_name,
            c.course_description,
            cs.schedule
        FROM
            my_class mc
        INNER JOIN
            course c ON mc.course_id = c.course_id
        INNER JOIN
            course_schedule cs ON c.course_id = cs.course_id;
    ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            My_Class currentMyClass = myclass.FirstOrDefault(mc => mc.class_id == Guid.Parse(reader["class_id"].ToString()));

                            if (currentMyClass == null)
                            {
                                currentMyClass = new My_Class
                                {
                                    class_id = Guid.Parse(reader["class_id"].ToString()),
                                    user_id = Guid.Parse(reader["user_id"].ToString()),
                                    course_id = Guid.Parse(reader["course_id"].ToString()),
                                    my_class = new List<Course>()
                                };

                                myclass.Add(currentMyClass);
                            }

                            if (currentMyClass != null)
                            {
                                currentMyClass.my_class.Add(new Course
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

            return myclass;
        }


        //SelectAllByCheckoutId
        public List<My_Class> GetAllByClass_id(Guid class_id)
        {
            List<My_Class> myclass = new List<My_Class>();

            string query = @"
    SELECT
            mc.class_id,
            mc.user_id,
            mc.course_id,
            c.category_id,
            c.course_name,
            c.course_description,
            cs.schedule
        FROM
            my_class mc
        INNER JOIN
            course c ON mc.course_id = c.course_id
        INNER JOIN
            course_schedule cs ON c.course_id = cs.course_id
        WHERE
            mc.class_id = @class_id
    ";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@class_id", class_id);
                    connection.Open();

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        My_Class currentMyClass = null;

                        while (reader.Read())
                        {

                            if (currentMyClass == null)
                            {
                                currentMyClass = new My_Class
                                {
                                    class_id = Guid.Parse(reader["class_id"].ToString()),
                                    user_id = Guid.Parse(reader["user_id"].ToString()),
                                    course_id = Guid.Parse(reader["course_id"].ToString()),
                                    my_class = new List<Course>()
                                };

                                myclass.Add(currentMyClass);
                            }

                            if (currentMyClass != null)
                            {
                                currentMyClass.my_class.Add(new Course
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

            return myclass;
        }

        //Insert MyClass
        public bool CreateMyClass(My_Class my_Class)
        {
            bool result = false;
            string query = @"
        INSERT INTO my_class (class_id, user_id, course_id)
        VALUES (@class_id, @user_id, @course_id)
    ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@class_id", my_Class.class_id);
                    command.Parameters.AddWithValue("@user_id", my_Class.user_id);
                    command.Parameters.AddWithValue("@course_id", my_Class.course_id);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0;

                    connection.Close();
                }
            }

            return result;
        }

        // UpdateMyClass
        public bool UpdateMyClass(Guid class_id, My_Class my_Class)
        {
            bool result = false;
            string query = @"
        UPDATE my_class
        SET user_id = @user_id,
            course_id = @course_id
        WHERE class_id = @class_id
    ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@user_id", my_Class.user_id);
                    command.Parameters.AddWithValue("@course_id", my_Class.course_id);
                    command.Parameters.AddWithValue("@class_id", class_id);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0;

                    connection.Close();
                }
            }

            return result;
        }

        // DeleteMyClass
        public bool DeleteMyClass(Guid class_id)
        {
            bool result = false;
            string query = @"
        DELETE FROM my_class
        WHERE class_id = @class_id
    ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@class_id", class_id);

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0;

                    connection.Close();
                }
            }

            return result;
        }
    }
}
