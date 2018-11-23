using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using ASPNetAuthEmail.Abstract;
using ASPNetAuthEmail.Models;

namespace ASPNetAuthEmail.Concrete
{
    public class EFOrderRepository: IOrderRepository
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<Order> Orders
        {
            get { return context.Orders; }
            set { ; }
        }

        // Сохранение
        public void SaveOrder(Order order)
        {
            if (order.OrderId == 0)
                context.Orders.Add(order);
            else
            {
                Order dbEntry = context.Orders.Find(order.OrderId);
                if (dbEntry != null)
                {
                    dbEntry.SurName = order.SurName;
                    dbEntry.Name = order.Name;
                    dbEntry.Otch = order.Otch;
                    dbEntry.Phone = order.Phone;
                    dbEntry.IdRealtor = order.IdRealtor;

                    //dbEntry.ImageData = game.ImageData;
                    //dbEntry.ImageMimeType = game.ImageMimeType;
                }
            }
            context.SaveChanges();
        }

        // Удаление
        public Order DeleteOrder(int orderId)
        {
            Order dbEntry = context.Orders.Find(orderId);
            if (dbEntry != null)
            {
                context.Orders.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

    }
}