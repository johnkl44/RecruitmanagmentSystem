using Microsoft.AspNetCore.Identity;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentManagementSystem.DAL;
using RecruitmentManagementSystem.Models;
using RecruitmentManagementSystem.Utilities;

namespace RecruitmentManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly Recruitment_DAL userDAL;
        public HomeController(Recruitment_DAL dal)
        {
            userDAL = dal;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        /// <summary>
        /// Sign in user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignIn(Users user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Invalid Credentials";
                }

                Users validUser = userDAL.GetUserByUsername(user.Username);

                if (validUser == null)
                {
                    TempData["ErrorMessage"] = "Invalid Username or Password.";
                    return View(user);
                }

                string decodedPassword = validUser.Decode(validUser.Password);
                if (decodedPassword == user.Password)
                {
                    HttpContext.Session.SetString("Role", validUser.Role);
                    HttpContext.Session.SetString("Username", validUser.Username);
                    HttpContext.Session.SetInt32("UserId", validUser.UserId);

                    return RedirectToAction(validUser.Role == "Admin" ? "AdminIndex" : "CandidateIndex", validUser.Role == "Admin" ? "Admin" : "Candidate");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid Username or Password.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                TempData["errorMessage"] = $"An error occurred: {ex.Message}";
                return View(user);
            }
        }

        /// <summary>
        /// Sign up user
        /// </summary>
        /// <returns></returns>
        // GET: SignUp
        [HttpGet]
        public IActionResult SignUp()
        {
            try
            {
                //var states = userDAL.GetStates();
                //ViewBag.States = states;
                //ViewBag.Cities = new List<SelectListItem>();

                return View();
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToAction("Error");
            }
        }
        [HttpPost]
        public IActionResult SignUp(Users userSignUp)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var states = userDAL.GetStates();
                    //ViewBag.States = states;
                    //ViewBag.Cities = new List<SelectListItem>();

                    TempData["errorMessage"] = "Details not Valid";
                    return View();
                }
                userSignUp.Password = userSignUp.Encode(userSignUp.Password);
                userDAL.SignUpUser(userSignUp, "Candidate", HttpContext);

                TempData["successMessage"] = "Registration Successful";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                //var states = userDAL.GetStates();
                //ViewBag.States = states;
                //ViewBag.Cities = new List<SelectListItem>(); 
                return View(userSignUp);
            }
        }
        /// <summary>
        /// Drop down state and cities
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public IActionResult GetCitiesByState(int stateId)
        {
            try
            {
                var cities = userDAL.GetCities(stateId);
                return View(cities);
            }
            catch (Exception ex)
            {
                return View(new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetStates()
        {
            var stateList = new Recruitment_DAL().GetStates(); 
            return Json(stateList);
        }


        [HttpGet]
        public IActionResult GetCities(int stateId)
        {
            try
            {
                var cityList = userDAL.GetCities(stateId);
                return Json(cityList);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }); 
            }
        }
        [HttpGet]
        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactUsModel contact)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Details not Valid";
                    return View();
                }
                userDAL.ContactUs(contact);

                TempData["successMessage"] = "Your Message Sent Successfully!";
                return RedirectToAction("ContactUS");
            }
            catch (Exception ex)
            {
                ExceptionLogging.SendErrorToText(ex, HttpContext);
                TempData["errorMessage"] = "An unexpected error occurred. Please try again later.";
                return View();
            }

        }
            
    }
}



