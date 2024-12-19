using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Mono.TextTemplating;
using NuGet.Protocol.Plugins;
using RecruitmentManagementSystem.Models;

namespace RecruitmentManagementSystem.DAL
{
    public class Recruitment_DAL
    {
        SqlConnection? conn;
        SqlCommand? cmd;
        public static IConfiguration Configuration { get; set; } = new ConfigurationBuilder().Build();
        private string? GetConnectionString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            return Configuration.GetConnectionString("DefaultConncetion");
        }
        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        public List<Users> GetAllUsers()
        {
            List<Users> userList = new List<Users>();
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SPR_Users";
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Users user = new Users();
                        user.UserId = Convert.ToInt32(dr["UserId"]);
                        user.FirstName = dr["FirstName"].ToString();
                        user.LastName = dr["LastName"].ToString();
                        user.DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]);
                        user.Gender = dr["Gender"].ToString();
                        user.PhoneNumber = dr["PhoneNumber"].ToString();
                        user.Email = dr["Email"].ToString();
                        user.Address = dr["Address"].ToString();
                        user.State = dr["State"].ToString();
                        user.City = dr["City"].ToString();
                        user.Username = dr["Username"].ToString();
                        user.Password = dr["Password"].ToString();
                        user.Role = dr["Role"].ToString();
                        userList.Add(user);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error occurred: {exception.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            return userList;
        }
        /// <summary>
        /// Register User
        /// </summary>
        /// <param name="model"></param>
        public void SignUpUser(Users model, string role)
        {
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SPI_User";
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@State", model.State);
                    cmd.Parameters.AddWithValue("@City", model.City);
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    cmd.Parameters.AddWithValue("@Role", role);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error occurred: {exception.Message}");
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// Get States form DtatBase
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetStates()
        {
            var states = new List<SelectListItem>();
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_GetState";
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            states.Add(new SelectListItem
                            {
                                Value = reader["StateId"].ToString(),
                                Text = reader["StateName"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    throw;
                }
            }
            return states;
        }
        /// <summary>
        /// Get CIties List based on State
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetCities(int stateId)
        {
            var cities = new List<SelectListItem>();
            using (var conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    var cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_CityDropDown";
                    cmd.Parameters.AddWithValue("@StateId", stateId);

                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cities.Add(new SelectListItem
                            {
                                Value = reader["CityId"].ToString(),
                                Text = reader["CityName"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching cities: " + ex.Message);
                }
            }
            return cities;
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="model"></param>
        public void UpdateUser(Users model)
        {
            try
            {
                using (conn = new SqlConnection(GetConnectionString()))
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SPU_User";
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", model.LastName);
                    cmd.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Address", model.Address);
                    cmd.Parameters.AddWithValue("@State", model.State);
                    cmd.Parameters.AddWithValue("@City", model.City);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }
        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="model"></param>
        public void ChangePassword(Users model)
        {
            try
            {
                using (conn = new SqlConnection(GetConnectionString()))
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "ChangePassword";
                    cmd.Parameters.AddWithValue("@UserId", model.UserId);
                    cmd.Parameters.AddWithValue("@Password", model.Password);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Deactivate user
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SPD_User";
                    cmd.Parameters.AddWithValue("@UserId", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        public Users GetUserById(int id)
        {
            Users? userID = null;
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_GetUserByID";
                    cmd.Parameters.AddWithValue("@UserId", id);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        userID = new Users
                        {
                            UserId = Convert.ToInt32(dr["UserId"]),
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                            DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]),
                            Gender = dr["Gender"].ToString(),
                            PhoneNumber = dr["PhoneNumber"].ToString(),
                            Email = dr["Email"].ToString(),
                            Address = dr["Address"].ToString(),
                            State = dr["State"].ToString(),
                            City = dr["City"].ToString(),
                            Username = dr["Username"].ToString(),
                            Password = dr["Password"].ToString(),
                            Role = dr["Role"].ToString(),
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            return userID;
        }

        /// <summary>
        /// Get User by name
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Users? GetUserByUsername(string username)
        {
            Users? user = null;
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_GetUserByUsername";
                    cmd.Parameters.AddWithValue("@UserName", username);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        user = new Users
                        {
                            UserId = Convert.ToInt32(dr["UserId"]),
                            Username = dr["Username"].ToString(),
                            FirstName = dr["FirstName"].ToString(),
                            LastName = dr["LastName"].ToString(),
                        };
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error occurred: {exception.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            return user;
        }

        ///<summary>
        ///for Validate User
        /// </summary>
        public string ValidateUser(string username, string password)
        {
            string? role = string.Empty;
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_ValidateUser";
                        cmd.Parameters.AddWithValue("@UserName", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        conn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                role = dr["Role"]?.ToString() ?? string.Empty;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during user validation: {ex.Message}");
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
            return role;
        }
        /// <summary>
        /// Creating New Jobs
        /// </summary>
        /// <param name="job"></param>
        public void JobCreation(JobCreationsModel job)
        {
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_JobCreation";
                    cmd.Parameters.AddWithValue("@JobTitle", job.JobTitle);
                    cmd.Parameters.AddWithValue("@JobDescription", job.JobDescription);
                    cmd.Parameters.AddWithValue("@RequiredSkills", job.RequiredSkills);
                    cmd.Parameters.AddWithValue("@Experience", job.Experience);
                    cmd.Parameters.AddWithValue("@SalaryRange", job.SalaryRange);
                    cmd.Parameters.AddWithValue("@Deadline", job.Deadline);
                    cmd.Parameters.AddWithValue("@JobStatus", "Active");
                    cmd.Parameters.AddWithValue("@Author", job.Author);
                    cmd.Parameters.AddWithValue("@PosterPhoto", job.Poster);
                    cmd.Parameters.AddWithValue("@PostingDate", job.PostingDate);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error occurred: {exception.Message}");
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// Retriving all jobs 
        /// </summary>
        /// <returns></returns>
        public List<JobCreationsModel> GetAllJobs()
        {
            List<JobCreationsModel> jobList = new List<JobCreationsModel>();
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SPR_JobPostings";
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        JobCreationsModel jobs = new JobCreationsModel();
                        jobs.JobId = Convert.ToInt32(dr["JobId"]);
                        jobs.JobTitle = dr["JobTitle"].ToString();
                        jobs.JobDescription = dr["JobDescription"].ToString();
                        jobs.RequiredSkills = dr["RequiredSkills"].ToString();
                        jobs.Experience = dr["Experience"].ToString();
                        jobs.SalaryRange = dr["SalaryRange"].ToString();
                        jobs.Deadline = Convert.ToDateTime(dr["Deadline"]);
                        jobs.JobStatus = dr["JobStatus"].ToString();
                        jobs.Author = Convert.ToInt32(dr["Author"]);
                        if (dr["PosterPhoto"] != DBNull.Value)
                        {
                            var photoData = (byte[])dr["PosterPhoto"];
                            jobs.PosterPhotoBase64 = Convert.ToBase64String(photoData);
                        }
                        else
                        {
                            jobs.PosterPhotoBase64 = null;
                        }
                        //jobs.PostingDate = dr["PostingDate"] != DBNull.Value ? Convert.ToDateTime(dr["PostingDate"]) : DateTime.MinValue;
                        jobList.Add(jobs);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error occurred: {exception.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            return jobList;
        }
        /// <summary>
        /// Get job by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JobCreationsModel GetJobById(int id)
        {
            JobCreationsModel? job = null;
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_GetJobByID";
                    cmd.Parameters.AddWithValue("@JobId", id);
                    conn.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        job = new JobCreationsModel
                        {
                            JobId = Convert.ToInt32(dr["JobId"]),
                            JobTitle = Convert.ToString(dr["JobTitle"]),
                            JobDescription = Convert.ToString(dr["JobDescription"]),
                            JobStatus = Convert.ToString(dr["JobStatus"]),
                            RequiredSkills = Convert.ToString(dr["RequiredSkills"]),
                            Experience = Convert.ToString(dr["Experience"]),
                            SalaryRange = Convert.ToString(dr["SalaryRange"]),
                            Deadline = Convert.ToDateTime(dr["Deadline"]),

                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
            return job;
        }
        /// <summary>
        ///  Applying application
        /// </summary>
        /// <param name="job"></param>
        public void ApplyApplication(ApplicationModel application)
        {
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_JobCreation";
                    cmd.Parameters.AddWithValue("@CandidateId", application.CandidateId);
                    cmd.Parameters.AddWithValue("@AppliedDate", application.AppliedDate);
                    cmd.Parameters.AddWithValue("@Resume", application.Resume);
                    cmd.Parameters.AddWithValue("@Photo", application.Photo);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error occurred: {exception.Message}");
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// Update jobs in admin
        /// </summary>
        /// <param name="jobs"></param>
        public void UpdateJobs(JobCreationsModel jobs)
        {
            try
            {
                using (conn = new SqlConnection(GetConnectionString()))
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SPU_Jobs";
                    cmd.Parameters.AddWithValue("@JobId", jobs.JobId);
                    cmd.Parameters.AddWithValue("@JobDescription", jobs.JobDescription);
                    cmd.Parameters.AddWithValue("@RequiredSkills", jobs.RequiredSkills);
                    cmd.Parameters.AddWithValue("@Experience", jobs.Experience);
                    cmd.Parameters.AddWithValue("@SalaryRange", jobs.SalaryRange);
                    cmd.Parameters.AddWithValue("@Deadline", jobs.Deadline);
                    cmd.Parameters.AddWithValue("@PosterPhoto", jobs.Poster);



                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");

            }
            finally
            {
                conn.Close();
            }
        }
        public void DeleteJob(int id)
        {
            using (conn = new SqlConnection(GetConnectionString()))
            {
                try
                {
                    cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SPD_JobPostings";
                    cmd.Parameters.AddWithValue("@JobId", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    //conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
