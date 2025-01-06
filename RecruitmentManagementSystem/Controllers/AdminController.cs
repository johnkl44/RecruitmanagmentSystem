using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementSystem.Models;
using RecruitmentManagementSystem.DAL;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using System.Reflection.Metadata.Ecma335;
using RecruitmentManagementSystem.Utilities;

namespace RecruitmentManagementSystem.Controllers
{
    public class AdminController : Controller
    {

        private readonly Recruitment_DAL adminDAL;
        public AdminController(Recruitment_DAL dal)
        {
            adminDAL = dal;
        }
        /// <summary>
        /// Admin home page
        /// </summary>
        /// <returns></returns>
        public IActionResult AdminIndex()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index","Home");
            }
           
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            return View();
        }
        /// <summary>
        /// logout form admin
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            TempData["successMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index","Home");
        }
        /// <summary>
        /// creating jobs in admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CreateJob()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View();
        }

        [HttpPost]
        public IActionResult CreateJob(JobCreationsModel job, IFormFile? PosterPhoto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Invalid credentials";
                    return View(job);
                }

                if (PosterPhoto != null && PosterPhoto.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        PosterPhoto.CopyTo(memoryStream);
                        job.Poster = memoryStream.ToArray();
                    }
                }

                var username = HttpContext.Session.GetString("Username");
                var userId = HttpContext.Session.GetInt32("UserId");

                if (string.IsNullOrEmpty(username) || !userId.HasValue)
                {
                    TempData["errorMessage"] = "Session expired or invalid. Please log in again.";
                    return RedirectToAction("SignIn", "Home");
                }

                job.Author = userId.Value;
                job.PostingDate = DateTime.UtcNow;

                adminDAL.JobCreation(job);

                TempData["successMessage"] = "Job successfully created";
                return RedirectToAction("AdminIndex");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(job);
            }
        }
        /// <summary>
        /// list all users in admin
        /// </summary>
        /// <returns></returns>
        public IActionResult Candidates()
        {
            List<Users> users = new List<Users>();  
            try
            {
                users = adminDAL.GetAllUsers();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
            }
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View(users);
        }
        /// <summary>
        /// list all jobs in admin and Perform CURD operations
        /// </summary>
        /// <returns></returns>
        public IActionResult ManageJobs()
        {
            List<JobCreationsModel> jobs = new List<JobCreationsModel>();
            try
            {
                jobs = adminDAL.GetAllJobs();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
            }
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View(jobs);
        }
        /// <summary>
        /// Add new admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddAdminUser()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View();
        }
        [HttpPost]
        public IActionResult AddAdminUser(Users adminUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Details not Valid";
                }
                adminDAL.SignUpUser(adminUser,"Admin",HttpContext);
                TempData["successMessage"] = "Registration Successfull";
                return RedirectToAction("AdminIndex");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View();
            }
        }
        /// <summary>
        /// Deactivate User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeactivateUser(int id)
        {
            try
            {
                Users user = adminDAL.GetUserById(id);
                if (user == null)
                {
                    TempData["errorMessage"] = $"User with ID {id} not found.";
                    return RedirectToAction("Candidates");
                }
                var username = HttpContext.Session.GetString("Username");
                ViewBag.Username = username;
                return View(user);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("Candidates");
            }
        }

        [HttpPost]
        [ActionName("DeactivateUser")]
        public IActionResult DeactivateUserConfirmation(int Id)
        {
            try
            {
                Users user = adminDAL.GetUserById(Id);

                adminDAL.Delete(user.UserId);
                TempData["successMessage"] = "User deactivated successfully.";
                return RedirectToAction("Candidates");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("Candidates");
            }
        }
        /// <summary>
        /// Update Jobs
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UpdateJobs(int id)
        {
            try
            {
                JobCreationsModel job = adminDAL.GetJobById(id);
                if (job == null)
                {
                    TempData["errorMessage"] = $"Job with ID {id} not found.";
                    return RedirectToAction("ManageJobs");
                }
                if (job.Poster != null && job.Poster.Length > 0)
                {
                    job.PosterPhotoBase64 = Convert.ToBase64String(job.Poster);
                }
                var username = HttpContext.Session.GetString("Username");
                ViewBag.Username = username;
                return View(job);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("ManageJobs");
            }
        }
        [HttpPost]
        public IActionResult UpdateJobs(JobCreationsModel job)
        {
            try
            {
               
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Invalid credentials";
                    return View(job);
                }

                if (job.PosterPhoto != null && job.PosterPhoto.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        job.PosterPhoto.CopyTo(memoryStream);
                        job.Poster = memoryStream.ToArray();
                    }
                }
                adminDAL.UpdateJobs(job, HttpContext);// HttpContext for getting the error logs
                TempData["successMessage"] = "Job successfully updated";
                return RedirectToAction("ManageJobs");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(job);
            }
        }
        /// <summary>
        /// Delete Job 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult DeleteJob(int id)
        {
            try
            {
                JobCreationsModel job = adminDAL.GetJobById(id);
                if (job == null)
                {
                    TempData["errorMessage"] = $"Job with ID {id} not found.";
                    return RedirectToAction("ManageJobs");
                }
                var username = HttpContext.Session.GetString("Username");
                ViewBag.Username = username;
                return View(job);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("ManageJobs");
            }
        }

        [HttpPost]
        [ActionName("DeleteJob")]
        public IActionResult DeleteJobConfirmation(int Id)
        {
            try
            {
                JobCreationsModel job = adminDAL.GetJobById(Id);

                adminDAL.DeleteJob(job.JobId);
                TempData["successMessage"] = "Job deactivated successfully.";
                return RedirectToAction("ManageJobs");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("ManageJobs");
            }
        }
        /// <summary>
        /// Settings Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Settings()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View();
        }
        /// <summary>
        /// Change admin Password
        /// </summary>
        /// <returns></returns>
        public IActionResult ChangePassword()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["errorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Login", "Home");
            }

            if ( string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["errorMessage"] = "All fields are required.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                TempData["errorMessage"] = "New password and confirmation password do not match.";
                return View();
            }

            try
            {
                var user = adminDAL.GetUserById((int)userId);

                if (user == null)
                {
                    TempData["errorMessage"] = "User not found.";
                    return RedirectToAction("SignIn", "Home");
                }

                //if (user.Password != oldPassword) 
                //{
                //    TempData["errorMessage"] = "Old password is incorrect.";
                //    return View();
                //}
                
                string encodedNewPassword = user.Encode(newPassword);
                user.Password = encodedNewPassword;
                adminDAL.ChangePassword(user);

                TempData["successMessage"] = "Password changed successfully!";
                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View();
            }
        }
        /// <summary>
        /// Get all applications
        /// </summary>
        /// <returns></returns>
        public IActionResult Applications()
        {
            try
            {
                var applications = adminDAL.GetAllApplications();
                var username = HttpContext.Session.GetString("Username");
                ViewBag.Username = username;
                return View(applications);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(new List<ApplicationViewModel>());
            }
        }
        /// <summary>
        /// Approve or reject the application 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ApplicationDetails(int id)
        {
            var application = adminDAL.GetApplicationByID(id);
            return View(application);
        }
        [HttpPost]
        public IActionResult UpdateApplicationStatus(int id, string status)
        {
            try
            {
                adminDAL.UpdateApplicationStatus(id, status);
                TempData["successMessage"] = "Application status updated successfully.";
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
            }
            return RedirectToAction("Applications");
        }
    }
}
