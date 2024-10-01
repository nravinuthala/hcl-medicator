using DAL;
using hcl_medicator.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace hcl_medicator.Controllers
{
    [NoCache]
    public class CustomerController : Controller
    {
        Registration reg;
        CustomerRepo repo;

        public CustomerController()
        {
            reg = new Registration();
            repo = new CustomerRepo();
        }

        public ActionResult CustomerSignup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerSignup(FormCollection form)
        {
            User userEntity = new User
            {
                Username = form["Username"],
                PasswordHash = ComputeSha256Hash(form["PasswordHash"]),
                Email = form["Email"],
                UserType = "Customer",
                Address = form["Address"],
            };

            if (reg.Signup(userEntity))
            {
                ViewBag.message = "User Created";
                return RedirectToAction("CustomerLogin");
            }
            else
            {
                ViewBag.message = "User Not Created";
                return View();
            }
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public ActionResult CustomerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CustomerLogin(FormCollection form)
        {
            User userEntity = new User
            {
                Username = form["Username"],
                PasswordHash = ComputeSha256Hash(form["PasswordHash"])
            };

            User user = reg.Login(userEntity);

            if (user != null)
            {
                Session["UserID"] = user.UserID; 
                Session["UserType"] = user.UserType;
                Session["Username"] = user.Username;
                return RedirectToAction("ViewAll", "Prescription");
            }
            else
            {
                ViewBag.Message = "Login Failed";
                return RedirectToAction("CustomerLogin");
            }
        }
            
        

        [NoCache]
        public ActionResult CustomerLogout()
        {
            Session.Clear(); 
            Session.Abandon();
            return RedirectToAction("CustomerLogin");
        }

        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize] 
        public ActionResult CustomerProfile()
        {
            int userId = Convert.ToInt32(Session["UserID"]); 
            User user = repo.GetUserById(userId); 

            if (user != null)
            {

                hcl_medicator.Models.UserModel userModel = new hcl_medicator.Models.UserModel
                {
                    Username = user.Username,
                    Email = user.Email,
                    UserType = user.UserType,
                    Address = user.Address,
                    
                };

                return View(userModel); 
            }
            else { return View(); }
           
        }

    }
}