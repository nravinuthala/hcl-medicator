using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hcl_medicator.Models;
using DAL; 
using hcl_medicator.Filters;

namespace hcl_medicator.Controllers
{
    [NoCache]
    public class ProductController : Controller
    {
        ProductRepo repo;
        public ProductController()
        {
            repo = new ProductRepo();
        }

        // GET: Product
        [CustomAuthorize]
        [NoCache]
        public ActionResult GetAllProducts()
        {
            
            List<Product> lpEntity = repo.ListProduct();
           
            List<ProductModel> productModel = new List<ProductModel>();
            foreach (var item in lpEntity)
            {
                ProductModel pm = new ProductModel();
                
                pm.Name = item.Name;
                pm.Description = item.Description;
                pm.Price = item.Price;
                pm.ProductID = item.ProductID;
                pm.QuantityAvailable = item.QuantityAvailable;
                pm.SellerID = item.SellerID;

               
                productModel.Add(pm);
            }

            
            return View(productModel);
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
            product.SellerID = model.SellerID;
            product.QuantityAvailable = model.QuantityAvailable;

            if (repo.SaveProduct(product))
            {
                ViewBag.message = "Product Has been Added Successfully";
            }
            else
            {
                ViewBag.message = "Product was not Added";
            }


            return View();
        }

        // GET: Delete Product
        [CustomAuthorize]
        [NoCache]
        public ActionResult DeleteProduct(int id)
        {
            var productEntity = repo.ListProductByID(id);
            if (productEntity == null)
            {
                return HttpNotFound();
            }

           
            var productModel = new ProductModel
            {
                ProductID = productEntity.ProductID,
                Name = productEntity.Name,
                Description = productEntity.Description,
                Price = productEntity.Price,
                QuantityAvailable = productEntity.QuantityAvailable,
                SellerID = productEntity.SellerID
            };

            return View(productModel); 
        }

        // POST: Delete Product
        
        [HttpPost]
        [NoCache]
        public ActionResult DeleteProduct(int id, FormCollection collection)
        {
            try
            {
                if (repo.DeleteProduct(id))
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
        [CustomAuthorize]
        [NoCache]
        public ActionResult Index()
        {
            return View();
        }
    }
}
