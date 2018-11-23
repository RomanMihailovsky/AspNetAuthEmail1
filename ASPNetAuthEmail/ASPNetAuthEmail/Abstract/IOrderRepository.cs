using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ASPNetAuthEmail.Models;

namespace ASPNetAuthEmail.Abstract
{
    public interface IOrderRepository
    {
        IEnumerable<Order> Orders { get; }
        void SaveOrder(Order order);
        Order DeleteOrder(int orderId);
    }
}
