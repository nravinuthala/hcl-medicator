using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL;
using hcl_medicator.Filters;
using hcl_medicator.Models;

namespace hcl_medicator.Controllers
{
    public class OrderController : Controller
    {
        OrderRepo repo;
        CartRepo cart;
        CustomerRepo customer;

        public OrderController()
        {
            repo = new OrderRepo();
            cart = new CartRepo();
            customer = new CustomerRepo();
        }

        [CustomAuthorize]
        public ActionResult ViewOrders()
        {
            int userId = (int)Session["UserID"];
            var o = repo.getOrder(userId);
            List<OrderModel> orders = new List<OrderModel>();
            foreach(var item in o)
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

                orders.Add(om);
            }
            return View(orders);
        }

        [CustomAuthorize]
        public ActionResult ViewAllOrders()
        {
            var o = repo.getOrders();
            List<OrderModel> orders = new List<OrderModel>();
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
                om.Address= item.Address;

                orders.Add(om);
            }
            return View(orders);
        }

        [CustomAuthorize]
        public ActionResult Delete(int orderId)
        {
            var o = repo.findOrder(orderId);
            OrderModel om = new OrderModel
            {
                OrderID = o.OrderID,
                Quantity = o.Quantity,
                Status = o.Status,
                Price = o.Price,
                TotalPrice = o.TotalPrice,
                UserID = o.UserID,
                SellerID = o.SellerID,
                CartID = o.CartID,
                ProductId = o.ProductId,
                ProductName = o.ProductName,
                Date = o.Date,
                Address = o.Address,

            };
            return View(om);

        }

        [HttpPost, ActionName("Delete")]
        [CustomAuthorize]
        public ActionResult DeleteConfirmed(int orderId)
        {
            var o = repo.findOrder(orderId);
            repo.DeleteOrder(orderId);
            return RedirectToAction("ViewAllOrders");
        }

        [HttpPost]
        [CustomAuthorize]
        public ActionResult PlaceOrder(int[] cartIds)
        {
            User user = customer.GetUserById((int)Session["UserID"]);
            try
            {

            
            foreach (var cartId in cartIds)
            {
                var c = cart.findCart(cartId);
                Order om = new Order
                {
                    CartID = c.CartID,
                    UserID = c.UserID,
                    ProductId = c.ProductID,
                    ProductName = c.ProductName,
                    Quantity = c.Quantity,
                    Price = c.Price,
                    TotalPrice = (decimal)c.Quantity * c.Price,
                    Date = DateTime.ParseExact(DateTime.Today.ToString("dd-MM-yyyy"),"dd-MM-yyyy",null),
                    Status = "Pending",
                    SellerID = c.SellerID,
                    Address = user.Address,

                };
                repo.AddOrder(om);
            }
            return RedirectToAction("ViewOrders");
        }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                    }
                }
                throw; 
            }
        }


        [CustomAuthorize]
        public ActionResult CancelOrder(int orderId)
        {
            var o = repo.findOrder(orderId);
            OrderModel om = new OrderModel
            {
                OrderID = o.OrderID,
                Quantity = o.Quantity,
                Status = o.Status,
                Price = o.Price,
                TotalPrice = o.TotalPrice,
                UserID = o.UserID,
                SellerID = o.SellerID,
                CartID = o.CartID,
                ProductId = o.ProductId,
                ProductName = o.ProductName,
                Date = o.Date,
                Address = o.Address,

            };
            return View(om);

        }
        [HttpPost, ActionName("CancelOrder")]
        [CustomAuthorize]
        public ActionResult CancelConfirmed(int orderId)
        {
            var o = repo.findOrder(orderId);
            if (repo.CancelOrder(orderId))
            {
                ViewBag.cancel = "Canceled Successfully";
                return RedirectToAction("ViewOrders");
            }
            ViewBag.cancel = "request for cancellation is sent!";
            return RedirectToAction("ViewOrders");
        }





    }
}