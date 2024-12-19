using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecruitmentManagementSystem.DAL;
using RecruitmentManagementSystem.Models;

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
                    TempData["errorMessage"] = "User not found.";
                    return View(user);
                }

                var role = userDAL.ValidateUser(user.Username,user.Password);
                if (!string.IsNullOrEmpty(role))
                {
                    HttpContext.Session.SetString("Role", role);
                    HttpContext.Session.SetString("Username", validUser.Username); 
                    HttpContext.Session.SetInt32("UserId", validUser.UserId); 

                    return RedirectToAction(role == "Admin" ? "AdminIndex" : "CandidateIndex", role == "Admin" ? "Admin" : "Candidate");
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid Username or Password.";
                    return View(user);
                }
            }
            catch (Exception ex)
            {
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
                TempData["errorMessage"] = $"Error loading signup page: {ex.Message}";
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
                userDAL.SignUpUser(userSignUp, "Candidate");

                TempData["successMessage"] = "Registration Successful";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;

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
                var cityList = userDAL.GetCities(stateId); // Fetch cities from DAL
                return Json(cityList); // Return as JSON
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }); // Return error as JSON if something goes wrong
            }
        }

        public IActionResult ContactUs()
        {
            return View();
        }

       

    }
}



