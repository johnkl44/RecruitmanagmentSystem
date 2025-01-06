using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementSystem.DAL;
using RecruitmentManagementSystem.Models;
using RecruitmentManagementSystem.Utilities;

namespace RecruitmentManagementSystem.Controllers
{
    public class CandidateController : Controller
    {
        private readonly Recruitment_DAL candidateDAL;
       
        public CandidateController(Recruitment_DAL dal)
        {
            candidateDAL = dal;
        }
        public IActionResult CandidateIndex()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            List<JobCreationsModel> jobs = new List<JobCreationsModel>();
            try
            {
                jobs = candidateDAL.GetAllJobs();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
            }
            return View(jobs);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            TempData["successMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Applications()
        {
            List<JobCreationsModel> jobs = new List<JobCreationsModel>();
            try
            {
                jobs = candidateDAL.GetAllJobs();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
            }
            return View(jobs);
        }
        /// <summary>
        /// Edit User Profile 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["errorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("Index", "Home");
            }
            var user = candidateDAL.GetUserById((int)userId);
            if(user == null)
            {
                TempData["ErrorMessage"] = "User not available";
                return RedirectToAction("CandidateIndex");
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult EditProfile(Users user)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["errorMessage"] = "Session expired. Please log in again.";
                    return RedirectToAction("Index", "Home");
                }
                
                user.UserId = (int)userId;
                candidateDAL.UpdateUser(user);
                TempData["successMessage"] = "Profile updated successfully.";
                return RedirectToAction("CandidateIndex");
            }
            catch (Exception exception)
            {
                ExceptionLogging.SendErrorToText(exception, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(user);
            }
        }
        public IActionResult MyProfile()
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["errorMessage"] = "Session expired. Please log in again.";
                    return RedirectToAction("Index", "Home");
                }

                var profile = candidateDAL.GetUserById((int)userId);
                if (profile.UserId == 0)
                {
                    TempData["errorMessage"] = "Your profile was not found.";
                    return RedirectToAction("Index");
                }
                return View(profile);
            }
            catch (Exception exception)
            {
                ExceptionLogging.SendErrorToText(exception, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View();
            }
        }
        /// <summary>
        /// applying for job
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ApplyforJob(int id)
        {
            var job = candidateDAL.GetJobById(id);
            if (job == null)
            {
                TempData["errorMessage"] = "The job you are trying to apply for does not exist.";
                return RedirectToAction("Applications");
            }

            var application = new ApplicationModel
            {
                JobId = id
            };

            return View(application); 
        }

        [HttpPost]
        public IActionResult ApplyforJob(ApplicationModel application, IFormFile? ResumeFile, IFormFile? ProfilePhoto,int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Please fill out all required fields correctly.";
                    return View(application);
                }

                if (ResumeFile != null)
                {
                    if (ResumeFile.ContentType == "application/pdf") 
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            ResumeFile.CopyTo(memoryStream);
                            application.Resume = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid resume format. Please upload a PDF file.";
                        return View(application);
                    }
                }

                if (ProfilePhoto != null)
                {
                    if (ProfilePhoto.ContentType == "image/jpeg" || ProfilePhoto.ContentType == "image/png") 
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            ProfilePhoto.CopyTo(memoryStream);
                            application.Photo = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid photo format. Please upload a JPEG or PNG file.";
                        return View(application);
                    }
                }

                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["errorMessage"] = "Your session has expired. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                application.CandidateId = (int)userId;
                application.JobId = id;
                application.AppliedDate = DateTime.UtcNow;

                candidateDAL.ApplyApplication(application);

                TempData["successMessage"] = "Your application has been submitted successfully!";
                return RedirectToAction("Applications");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(application);
            }
        }

        /// <summary>
        /// Settings page view
        /// </summary>
        /// <returns></returns>
        public IActionResult Settings()
        {
            return View();
        }
        /// <summary>
        /// Change Candidate Password
        /// </summary>
        /// <returns></returns>
        //public IActionResult ChangePassword()
        //{
        //    var username = HttpContext.Session.GetString("Username");
        //    ViewBag.Username = username;
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");

        //    if (userId == null)
        //    {
        //        TempData["errorMessage"] = "Session expired. Please log in again.";
        //        return RedirectToAction("Login", "Account");
        //    }

        //    if ( string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        //    {
        //        TempData["errorMessage"] = "All fields are required.";
        //        return View();
        //    }

        //    if (newPassword != confirmPassword)
        //    {
        //        TempData["errorMessage"] = "New password and confirmation password do not match.";
        //        return View();
        //    }

        //    try
        //    {
        //        var user = candidateDAL.GetUserById((int)userId);

        //        if (user == null)
        //        {
        //            TempData["errorMessage"] = "User not found.";
        //            return RedirectToAction("SignIn", "Home");
        //        }
        //        string decodeOldPassword = user.Decode(oldPassword);
        //        if (user.Password != decodeOldPassword)
        //        {
        //            TempData["errorMessage"] = "Old password is incorrect.";
        //            return View();
        //        }

        //        string encodedNewPassword = user.Encode(newPassword);
        //        user.Password = encodedNewPassword;
        //        candidateDAL.ChangePassword(user);

        //        TempData["successMessage"] = "Password changed successfully!";
        //        return RedirectToAction("Settings");
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogging.SendErrorToText(ex, HttpContext);
        //        TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
        //        return View();
        //    }
        //}
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

            if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
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
                var user = candidateDAL.GetUserById((int)userId);

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
                candidateDAL.ChangePassword(user);

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

        private bool VerifyPassword(string storedHash, string inputPassword)
        {
            // This method will compare the stored hash with the hashed input password
            // Assuming you're using a hashing library like SHA-256 or bcrypt
            return storedHash == HashPassword(inputPassword);
        }

        private string HashPassword(string password)
        {
            // This method hashes the password (you can use a hashing algorithm like SHA-256, bcrypt, etc.)
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString(); // This returns the hashed password
            }
        }

        /// <summary>
        /// CAndidate Applications Submitted
        /// </summary>
        /// <returns></returns>
        public IActionResult MyApplications()
        {
            try
            {
                var candidateId = HttpContext.Session.GetInt32("UserId");
                if (candidateId == null)
                {
                    TempData["errorMessage"] = "Session expired. Please log in again.";
                    return RedirectToAction("Index");
                }

                var applications = candidateDAL.GetApplicationsByCandidateId((int)candidateId);

                if (applications.Count == 0)
                {
                    TempData["infoMessage"] = "You have not submitted any applications yet.";
                }

                return View(applications);
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("CandidateIndex");
            }
        }

    }
}
