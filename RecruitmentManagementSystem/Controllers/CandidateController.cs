using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementSystem.DAL;
using RecruitmentManagementSystem.Models;

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
                TempData["errorMessage"] = ex.Message;
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
                TempData["errorMessage"] = ex.Message;
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
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Data is invalid.";
                    return View(user);
                }
                user.UserId = (int)userId;
                candidateDAL.UpdateUser(user);
                TempData["successMessage"] = "Profile updated successfully.";
                return RedirectToAction("CandidateIndex");
            }
            catch (Exception exception)
            {
                TempData["errorMessage"] = exception.Message;
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
                TempData["errorMessage"] = exception.Message;
                return View();
            }
        }
        /// <summary>
        /// applying for job
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ApplyforJob(int jobId)
        {
            var job = candidateDAL.GetJobById(jobId);
            if (job == null)
            {
                TempData["errorMessage"] = "Job not found.";
                return RedirectToAction("Applications");
            }

            return View(job);
        }

        [HttpPost]
        public IActionResult ApplyforJob(ApplicationModel application, IFormFile? photo, IFormFile resume)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Please fill out all required fields correctly.";
                    return View(application);
                }

                if (resume != null)
                {
                    if (resume.Length > 0 && resume.ContentType == "application/pdf")
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            resume.CopyTo(memoryStream);
                            application.Resume = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid resume format. Please upload a PDF file.";
                        return View(application);
                    }
                }

                if (photo != null)
                {
                    if (photo.Length > 0 && (photo.ContentType == "image/jpeg" || photo.ContentType == "image/png"))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            photo.CopyTo(memoryStream);
                            application.Photo = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid photo format. Please upload a JPEG or PNG file.";
                        return View(application);
                    }
                }

                application.AppliedDate = DateTime.UtcNow;

                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    TempData["errorMessage"] = "User not logged in.";
                    return RedirectToAction("Login", "Account");
                }

                application.CandidateId = (int)userId; 

                candidateDAL.ApplyApplication(application);
               

                TempData["successMessage"] = "Your application was submitted successfully!";
                return RedirectToAction("ApplicationSuccess");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"An error occurred: {ex.Message}";
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
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["errorMessage"] = "Session expired. Please log in again.";
                return RedirectToAction("SignUp","Home");
            }

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
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

                if (user.Password != oldPassword)
                {
                    TempData["errorMessage"] = "Old password is incorrect.";
                    return View();
                }

                user.Password = newPassword;
                candidateDAL.ChangePassword(user);

                TempData["successMessage"] = "Password changed successfully!";
                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"An error occurred: {ex.Message}";
                return View();
            }
        }
    }
}
