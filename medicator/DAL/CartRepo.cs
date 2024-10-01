using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CartRepo
    {
        private medicatordbContext dbContext;

        public CartRepo()
        {
            dbContext = new medicatordbContext();
        }

        
        public bool AddToCart(ShoppingCart cartItem)
        {
            dbContext.ShoppingCarts.Add(cartItem); 
            return dbContext.SaveChanges() > 0; 
        }

        
        public List<ShoppingCart> ListCartItems(int userId)
        {
            return dbContext.ShoppingCarts.Where(cart => cart.UserID == userId).ToList();
        }

        
        public bool RemoveFromCart(int cartId)
        {
            var cartItem = dbContext.ShoppingCarts.FirstOrDefault(cart => cart.CartID == cartId);
            if (cartItem != null)
            {
                dbContext.ShoppingCarts.Remove(cartItem);
                return dbContext.SaveChanges() > 0;
            }
            return false;
        }

        public bool UpdateCartItem(ShoppingCart cartItem)
        {
            
            dbContext.Entry(cartItem).State = System.Data.Entity.EntityState.Modified;
            return dbContext.SaveChanges() > 0;
        }


        public ShoppingCart FindCartItem(int userId, int productId)
        {
            return dbContext.ShoppingCarts.FirstOrDefault(cart => cart.UserID == userId && cart.ProductID == productId);
        }

        public ShoppingCart findCart(int id)
        {
            return dbContext.ShoppingCarts.FirstOrDefault(x=>x.CartID == id);
        }


    }
}
