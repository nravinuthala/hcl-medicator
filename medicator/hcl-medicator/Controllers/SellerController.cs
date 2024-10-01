using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DAL;
using hcl_medicator.Filters;
using hcl_medicator.Models;

namespace hcl_medicator.Controllers
{
    [NoCache]
    
    public class SellerController : Controller
    {

        ProductRepo productRepo;
        Registration reg;
        CustomerRepo users;
        OrderRepo orders;
        public SellerController()
        {
            productRepo = new ProductRepo();
            reg = new Registration();
            users = new CustomerRepo();
            orders = new OrderRepo();
        }

        // GET: Seller
        [CustomAuthorize]
        public ActionResult ViewAll()
        {
            int sellerId = (int)Session["UserID"];
            var product = productRepo.getProductBySeller(sellerId);

            List<ProductModel> products = new List<ProductModel>();
            foreach (var item in product)
            {
                ProductModel pm = new ProductModel();

                pm.Name = item.Name;
                pm.Description = item.Description;
                pm.Price = item.Price;
                pm.ProductID = item.ProductID;
                pm.QuantityAvailable = item.QuantityAvailable;
                pm.SellerID = item.SellerID;

                products.Add(pm);
            }

            return View(products);
        }

        // POST: Product details
        [CustomAuthorize]
        [NoCache]
        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductModel model)
        {
            Product product = new Product();

            product.ProductID = model.ProductID;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Name = model.Name;
            product.SellerID = (int)Session["UserID"];
            product.QuantityAvailable = model.QuantityAvailable;

            if (productRepo.SaveProduct(product))
            {
                ViewBag.message = "Product Has been Added Successfully";
            }
            else
            {
                ViewBag.message = "Product was not Added";
            }


            return View();
        }

        // Logout
        [NoCache]
        public ActionResult Logout()
        {
            Session.Clear(); // Clears the session
            Session.Abandon();
            return RedirectToAction("Index", "Seller");
        }


        //Delete product
        [CustomAuthorize]
        [NoCache]
        public ActionResult DeleteProduct(int id)
        {
            var productEntity = productRepo.ListProductByID(id);
            if (productEntity == null)
            {
                return HttpNotFound();
            }

            // Convert DAL.Product to hcl_medicator.Models.ProductModel
            var productModel = new ProductModel
            {
                ProductID = productEntity.ProductID,
                Name = productEntity.Name,
                Description = productEntity.Description,
                Price = productEntity.Price,
                QuantityAvailable = productEntity.QuantityAvailable,
                SellerID = productEntity.SellerID
            };

            return View(productModel); // Pass the converted model to the view
        }

        // POST: Delete Product

        [HttpPost]
        [NoCache]
        public ActionResult DeleteProduct(int id, FormCollection collection)
        {
            try
            {
                if (productRepo.DeleteProduct(id))
                {
                    ViewBag.Message = "Product deleted successfully.";
                }
                else
                {
                    ViewBag.Message = "Product not found or could not be deleted.";
                }
                return RedirectToAction("GetAllProducts");
            }
            catch
            {
                return View();
            }
        }


        // Index
        [NoCache]
        public ActionResult Index()
        {
            return View();
        }


        // Signup
        public ActionResult SellerSignup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SellerSignup(FormCollection form)
        {
            User userEntity = new User
            {
                Username = form["Username"],
                PasswordHash = ComputeSha256Hash(form["PasswordHash"]),
                Email = form["Email"],
                UserType = "Seller"
            };

            if (reg.Signup(userEntity))
            {
                ViewBag.message = "User Created";
                return RedirectToAction("SellerLogin");
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

        // Login
        public ActionResult SellerLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SellerLogin(FormCollection form)
        {
            User userEntity = new User
            {
                Username = form["Username"],
                PasswordHash = ComputeSha256Hash(form["PasswordHash"])
            };

            User user = reg.Login(userEntity);

            if (user != null)
            {
                Session["UserID"] = user.UserID; // Use UserID or any unique identifier
                Session["UserType"] = user.UserType;
                return RedirectToAction("DashBoard");
            }
            else
            {
                ViewBag.Message = "Login Failed";
                return RedirectToAction("SellerLogin");
            }
        }

        [CustomAuthorize]
        public ActionResult SellerProfile()
        {
            int userId = Convert.ToInt32(Session["UserID"]);
            User user = users.GetUserById(userId);

            if (user != null)
            {

                hcl_medicator.Models.UserModel userModel = new hcl_medicator.Models.UserModel
                {
                    Username = user.Username,
                    Email = user.Email,
                    UserType = user.UserType,

                };

                return View(userModel);
            }
            else { return View(); }

        }

        [CustomAuthorize]
        public ActionResult Orders()
        {
            int userId = (int)Session["UserID"];
            var o = orders.getOrderSeller(userId);
            List<OrderModel> orderModel = new List<OrderModel>();
            foreach (var item in o)
            {
                OrderModel om = new OrderModel();
                om.OrderID = item.OrderID;
                om.Quantity = item.Quantity;
                om.Status = item.Status;
                om.TotalPrice = item.TotalPrice;
                om.Price = item.Price;
                om.ProductId = item.ProductId;
                om.ProductName = item.ProductName;
                om.CartID = item.CartID;
                om.UserID = item.UserID;
                om.SellerID = item.SellerID;
                om.Date = item.Date;
                om.Address = item.Address;

                orderModel.Add(om);
            }
            return View(orderModel);
        }

        [CustomAuthorize]
        public ActionResult DashBoard()
        {
            int userId = (int)Session["UserID"];
            var order = orders.getOrderSeller(userId);
            var products = productRepo.getProductBySeller(userId);

            var viewModel = new SellerDashboard
            {

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
                return RedirectToAction("Orders");
            }

        
            var orderToUpdate = orders.findOrder(orderId);
            if (orderToUpdate == null)
            {
                
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Orders");
            }

          
            orderToUpdate.Status = status;

         
            orders.update(orderToUpdate);

          
            TempData["Success"] = "Order status updated successfully.";

          
            return RedirectToAction("Orders");
        }

    }
}