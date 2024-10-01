using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Registration
    {
        // initialize DBContext
        medicatordbContext dbContext;
        public Registration() 
        {
            dbContext = new medicatordbContext();
        }

        public bool Signup(User user) 
        {
            dbContext.Users.Add(user);

            if(dbContext.SaveChanges()>0)
            {
                return true;

            }else { return false; }

        }

        public User Login(User user)
        {
            User userDetails = dbContext.Users.Where(x=>x.Username == user.Username && x.PasswordHash == user.PasswordHash).FirstOrDefault();

            return userDetails;
        }

        public List<User> GetAllUsers()
        {
            return dbContext.Users.ToList();
        }

    }
}
