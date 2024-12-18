﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RecruitmentManagementSystem.Models;
using RecruitmentManagementSystem.DAL;
using Microsoft.AspNetCore.Mvc.Core.Infrastructure;
using System.Reflection.Metadata.Ecma335;

namespace RecruitmentManagementSystem.Controllers
{

    public class AdminController : Controller
    {

        private readonly Recruitment_DAL adminDAL;
        public AdminController(Recruitment_DAL dal)
        {
            adminDAL = dal;
        }
        public IActionResult AdminIndex()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index","Home");
            }
           
            var username = HttpContext.Session.GetString("Username");
            var userId = HttpContext.Session.GetInt32("UserId");
            ViewBag.UserId = userId;
            ViewBag.Username = username;
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Clear();
            TempData["successMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public IActionResult CreateJob()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role) || !role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Home");
            }
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

                // Assign job author and posting date
                job.Author = userId.Value;
                job.PostingDate = DateTime.UtcNow;

                // Save the job to the database
                adminDAL.JobCreation(job);

                TempData["successMessage"] = "Job successfully created";
                return RedirectToAction("AdminIndex");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(job);
            }
        }

        public IActionResult Candidates()
        {
            List<Users> users = new List<Users>();  
            try
            {
                users = adminDAL.GetAllUsers();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View(users);
        }

        public IActionResult ViewJobs()
        {
            List<JobCreationsModel> jobs = new List<JobCreationsModel>();
            try
            {
                jobs = adminDAL.GetAllJobs();
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            return View(jobs);
        }
        /// <summary>
        /// Add new admin
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddAdminUser()
        {
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
                adminDAL.SignUpUser(adminUser,"Admin");
                TempData["successMessage"] = "Registration Successfull";
                return RedirectToAction("AdminIndex");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
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
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = $"Error occurred while deactivating user: {ex.Message}";
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
                TempData["errorMessage"] = ex.Message;
                return RedirectToAction("Candidates");
            }
        }


        /// <summary>
        /// Settings Page
        /// </summary>
        /// <returns></returns>
        public IActionResult Settings()
        {
            return View();
        }
        /// <summary>
        /// Change admin Password
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
                return RedirectToAction("Login", "Account");
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
                var user = adminDAL.GetUserById((int)userId);

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
                adminDAL.ChangePassword(user);

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
