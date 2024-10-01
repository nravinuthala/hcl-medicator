using DAL;
using hcl_medicator.Filters;
using hcl_medicator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hcl_medicator.Controllers
{
    [NoCache]
    public class ProductCatalogController : Controller
    {
        // GET: ProductCatalog
        ProductRepo repo;
        CartRepo cartRepo;

        public ProductCatalogController()
        {
            repo = new ProductRepo();
            cartRepo = new CartRepo();
        }

        // GET: Product
        [CustomAuthorize]
        [NoCache]
        public ActionResult Index(string search = "")
        {
            List<Product> lpEntity = repo.ListProduct();

            // If a search string is provided, filter the product list
            if (!string.IsNullOrWhiteSpace(search))
            {
                lpEntity = lpEntity.Where(p => p.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                                            || p.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }

            List<ProductModel> productModel = new List<ProductModel>();
            foreach (var item in lpEntity)
            {
                ProductModel pm = new ProductModel
                {
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    ProductID = item.ProductID,
                    QuantityAvailable = item.QuantityAvailable,
                    SellerID = item.SellerID
                };

                productModel.Add(pm);
            }

            return View(productModel);
        }



        [HttpPost]
        [CustomAuthorize]
        public ActionResult AddToCart(int productId, int quantity)
        {

            int userId = (int)Session["UserID"];


            ShoppingCart existingCartItem = cartRepo.FindCartItem(userId, productId);

            Product product = repo.ListProductByID(productId);

            if (existingCartItem != null)
            {

                existingCartItem.Quantity += quantity;
                bool updateSuccess = cartRepo.UpdateCartItem(existingCartItem);
                if (updateSuccess)
                {

                    return RedirectToAction("Index");
                }
                else
                {

                    return RedirectToAction("Error");
                }
            }
            else
            {

                ShoppingCart cartItem = new ShoppingCart
                {
                    UserID = userId,
                    ProductID = productId,
                    Quantity = quantity,
                    ProductName = product.Name,
                    Price = product.Price,
                    SellerID= product.SellerID,
                };

                bool addSuccess = cartRepo.AddToCart(cartItem);
                if (addSuccess)
                {

                    return RedirectToAction("Index");
                }
                else
                {

                    return RedirectToAction("Error");
                }
            }


        }

        [CustomAuthorize]
        public ActionResult ShowCart()
        {
            int userId = (int)Session["UserID"]; 

            
            var cartItems = cartRepo.ListCartItems(userId);

         
            var cartViewModels = cartItems.Select(item => new CartItemViewModel
            {
                Id = item.CartID,
                ProductID = item.ProductID,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                Price = item.Product.Price

            }).ToList();

            decimal cartTotal = cartViewModels.Sum(item => item.TotalPrice);


            ViewBag.CartTotal = cartTotal;

            return View(cartViewModels); 
        }

        [NoCache]
        public ActionResult Logout()
        {
            Session.Clear(); 
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }


        [CustomAuthorize]
        [NoCache]
        public ActionResult DeleteProduct(int productId, int cartId)
        {
            int userId = (int)Session["UserID"];
            var productEntity = cartRepo.FindCartItem(userId, productId);
            if (productEntity == null)
            {
                return HttpNotFound();
            }


            var productModel = new CartModel
            {
                CartID = productEntity.CartID,
                UserID = productEntity.UserID,
                ProductID = productEntity.ProductID,
                Quantity = productEntity.Quantity,
                ProductName = productEntity.ProductName,
                Product = productEntity.Product,
                User = productEntity.User,
                Price = productEntity.Product.Price
            };

            return View(productModel);
        }


        [CustomAuthorize]
        [HttpPost]
        [NoCache]
        public ActionResult DeleteProduct(int productId, int cartId, FormCollection collection)
        {
            try
            {
                if (cartRepo.RemoveFromCart(cartId))
                {
                    ViewBag.Message = "Product deleted successfully.";
                }
                else
                {
                    ViewBag.Message = "Product not found or could not be deleted.";
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }

        }

        [CustomAuthorize]
        public ActionResult Details(int id)
        {
            
            Product product = repo.ListProductByID(id);

            if (product == null)
            {
                
                return HttpNotFound();
            }

            
            var productModel = new ProductModel
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SellerID = product.SellerID,
                QuantityAvailable = product.QuantityAvailable
            };

            
            return View(productModel);
        }

        public ActionResult Edit(int id)
        {
            int userId = (int)Session["UserID"];
            var c = cartRepo.FindCartItem(userId, id);
            var cartModel = new CartModel
            {
                UserID = c.UserID,
                ProductID = c.ProductID,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                Price =c.Price,
                CartID = c.CartID,  
            };
            ViewBag.productName = c.ProductName.ToString();
            return View(cartModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public ActionResult Edit(ShoppingCart cartModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            cartRepo.UpdateCartItem(cartModel);
            return RedirectToAction("ShowCart");

        }
    }
}
