using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using hcl_medicator.Models;
using System.Security.Cryptography;
using System.Text;
using hcl_medicator.Filters;
using System.Globalization;

namespace hcl_medicator.Controllers
{
    [NoCache]
    public class AccountController : Controller
    {
        Registration reg;
        CustomerRepo repo;
        OrderRepo orders;
        ProductRepo product;

        public AccountController()
        {
            reg = new Registration();
            repo = new CustomerRepo();
            orders = new OrderRepo();
            product = new ProductRepo();
        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(FormCollection form)
        {
            User userEntity = new User
            {
                Username = form["Username"],
                PasswordHash = ComputeSha256Hash(form["PasswordHash"]),
                Email = form["Email"],
                UserType = form["UserType"],
                Address = form["Address"],
            };

            if (reg.Signup(userEntity))
            {
                ViewBag.message = "User Created";
                return RedirectToAction("Login");
            }
            else
            {
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
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

                if (user.UserType == "Admin")
                {
                    return RedirectToAction("DashBoard");
                }
                else
                {
                    ViewBag.Message = "Admin Login Failed";
                    return RedirectToAction("Login");
                }
            }
            else
            {
                ViewBag.Message = "Admin Login Failed";
                return View();
            }
        }

        [NoCache]
        public ActionResult Logout()
        {
            Session.Clear(); // Clears the session
            Session.Abandon();
            return RedirectToAction("Index","Home");
        }

        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public ActionResult ViewUsers()
        {

            // we cant use list to return in a view
            List<User> lpEntity = reg.GetAllUsers();
            // convert to model
            List<UserModel> userModel = new List<UserModel>();
            foreach (var item in lpEntity)
            {
                UserModel um = new UserModel();
                // add all the items received from DAL into productModel
                um.UserID = item.UserID;
                um.Username = item.Username;
                um.PasswordHash = item.PasswordHash;
                um.Email = item.Email;
                um.UserType = item.UserType;
                um.Address = item.Address;
                

                // add to productModel List
                userModel.Add(um);
            }

            // view will only return models
            return View(userModel);
        }

        [CustomAuthorize]
        public ActionResult AdminProfile()
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

        [CustomAuthorize]
        public ActionResult DeleteUser(int id) 
        {
            var user = repo.GetUserById(id);
            if (user != null)
            {
                var userModel = new UserModel
                {
                    Username = user.Username,
                    Email = user.Email,
                    UserType = user.UserType,
                    UserID = user.UserID,
                    PasswordHash = user.PasswordHash,

                };

                return View(userModel);
            }else { return View(); }
        }

        [CustomAuthorize]
        [HttpPost]
        [NoCache]
        public ActionResult DeleteUser(int id, FormCollection form)
        {
            try
            {
                if (repo.deleteUser(id))
                {
                    ViewBag.delete = "User Deleted Successfully";
                }else
                {
                    ViewBag.delete = "User not Deleted or Not Found";
                }
                return RedirectToAction("ViewUsers");
            }catch
            {
                return View();
            }
        }

        [CustomAuthorize]
        public ActionResult DashBoard()
        {
            var users = reg.GetAllUsers();
            var order = orders.getOrders();
            var products = product.ListProduct();

            var viewModel = new DashBoardModel
            {
                TotalUserCount = users.Count,
                TotalCustomerCount = users.Count(u => u.UserType == "Customer"),
                TotalSellerCount = users.Count(u => u.UserType == "Seller"),
                TotalOrdersPlaced = order.Count(),
                TotalOrdersDelivered = order.Count(o => o.Status == "Delivered"),
                TotalProductsListed = products.Count
            };

            return View(viewModel);
        }

        [CustomAuthorize]
        public ActionResult UpdateStatus(int orderId, string status)
        {

            if (status != "Confirmed" && status != "Delivered")
            {
                
                TempData["Error"] = "Invalid status.";
                return RedirectToAction("ViewAllOrders","Order");
            }

            
            var orderToUpdate = orders.findOrder(orderId);
            if (orderToUpdate == null)
            {
                
                TempData["Error"] = "Order not found.";
                return RedirectToAction("ViewAllOrders", "Order");
            }

           
            orderToUpdate.Status = status;


            orders.update(orderToUpdate);


            TempData["Success"] = "Order status updated successfully.";


            return RedirectToAction("ViewAllOrders", "Order");
        }
    }
}

    

    

