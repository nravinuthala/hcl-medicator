using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class OrderRepo
    {
        medicatordbContext db;
        public OrderRepo()
        {
            db = new medicatordbContext();
        }

        public IEnumerable<Order> getOrders()
        {
            return db.Orders.ToList();
        } 

        public IEnumerable<Order> getOrder(int id)
        {
            return db.Orders.Where(x => x.UserID == id).ToList();
        }

        public bool update(Order order)
        {
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;

            if (db.SaveChanges() > 0)
            {
                return true;

            }
            return false;
        }

        public bool AddOrder(Order order)
        {
            db.Orders.Add(order);
            if(db.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool DeleteOrder(int id)
        {
            var order = db.Orders.Find(id);
            if(order != null)
            {
                db.Orders.Remove(order);
                if(db.SaveChanges() > 0 )
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public bool CancelOrder(int id)
        {
            var order = db.Orders.Find(id);
            if (order != null && order.Status != "Confirmed")
            {
                db.Orders.Remove(order);
                if (db.SaveChanges() > 0)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public Order findOrder(int id)
        {
            return db.Orders.Find(id);
        }

        public List<Order> getOrderSeller(int id)
        {
            return db.Orders.Where(x => x.SellerID == id).ToList();
        }
    }
}
