using Language.Models;
using MySql.Data.MySqlClient;

namespace Language.Data
{
    public class ScheduleData
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public ScheduleData(IConfiguration configuration)
        {
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        //Select Schedule by course_id
        public List<Course_Schedule> GetScheduleByCourseId(Guid course_id)
        {
            List<Course_Schedule> schedules = new List<Course_Schedule>();

            string query = $"SELECT * FROM course_schedule " +
                $"WHERE course_id = @course_id";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using(MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@course_id", course_id);
                    connection.Open();

                    using(MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Course_Schedule schedule = new Course_Schedule
                            {
                                schedule_id = Guid.Parse(reader["schedule_id"].ToString()),
                                course_id = Guid.Parse(reader["course_id"].ToString()),
                                course_date = DateTime.Parse(reader["course_date"].ToString())
                            };
                            schedules.Add(schedule);
                        }
                    }

                }
            }
            return schedules;
        }

        //Create Schedule
        public bool CreateSchedule(Course_Schedule schedule)
        {
            bool result = false;
            string query = $"INSERT INTO course_schedule (schedule_id, course_id, course_date) " +
                     $"VALUES (@schedule_id, @course_id, @course_date)";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@schedule_id", schedule.schedule_id);
                    command.Parameters.AddWithValue("@course_id", schedule.course_id);
                    command.Parameters.AddWithValue("@course_date", schedule.course_date);


                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }
            return result;

        }

        //Update
        public bool Update(Guid schedule_id, Course_Schedule schedule)
        {
            bool result = false;



            string query = $"UPDATE course_schedule SET course_id = @course_id, course_date = @course_date WHERE schedule_id = @schedule_id";



            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.AddWithValue("@schedule_id", schedule_id);
                    command.Parameters.AddWithValue("@course_id", schedule.course_id);
                    command.Parameters.AddWithValue("@course_date", schedule.course_date);


                    command.Connection = connection;
                    command.CommandText = query;

                    connection.Open();

                    result = command.ExecuteNonQuery() > 0 ? true : false;

                    connection.Close();
                }
            }

            return result;
        }

        // Delete
        public bool Delete(Guid schedule_id)
        {
            bool result = false;

            string query = $"DELETE FROM course_schedule WHERE schedule_id = @schedule_id";


            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@schedule_id", schedule_id);

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
