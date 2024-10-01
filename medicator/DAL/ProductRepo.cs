using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ProductRepo
    {
        medicatordbContext dbContext;
        public ProductRepo()
        {
            dbContext = new medicatordbContext();
        }

        public bool SaveProduct(Product product)
        {
            dbContext.Products.Add(product); 
            if (dbContext.SaveChanges() > 0)
            {
                return true;

            }// saving changes into the database
            else { return false; }
        }

        public List<Product> ListProduct()
        {
            return dbContext.Products.ToList();
        }

        public Product ListProductByID(int id)
        {
            return dbContext.Products.Find(id);
        }

        public bool DeleteProduct(int id)
        {
            var product = dbContext.Products.FirstOrDefault(p => p.ProductID == id);
            if (product != null)
            {
                dbContext.Products.Remove(product);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<Product> getProductBySeller(int id)
        {
            var product = dbContext.Products.Where(p => p.SellerID == id).ToList();
            if (product != null)
            {
                return product;
            }
            return null;
        }


    }
}
