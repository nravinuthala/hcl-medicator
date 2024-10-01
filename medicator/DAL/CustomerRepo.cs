using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CustomerRepo
    {
        private medicatordbContext dbContext; 

        public CustomerRepo()
        {
            dbContext = new medicatordbContext(); 
        }

        // This function retrieves a User entity by its ID
        public User GetUserById(int userId)
        {
            
            var user = dbContext.Users.FirstOrDefault(u => u.UserID == userId);
            return user;
        }

        public bool deleteUser (int userId)
        {
            var user = dbContext.Users.FirstOrDefault (u => u.UserID == userId);
            if (user != null)
            {
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}

