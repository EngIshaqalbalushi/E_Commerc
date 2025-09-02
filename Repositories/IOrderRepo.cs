using E_CommerceSystem.Models;
using System.Collections.Generic;

namespace E_CommerceSystem.Repositories
{
    public interface IOrderRepo
    {
        void AddOrder(Order order);
        void DeleteOrder(int oid);
        IEnumerable<Order> GetAllOrders();
        Order GetOrderById(int oid);
        void UpdateOrder(Order order);

        // ✅ keep only one version of this
        IEnumerable<Order> GetOrderByUserId(int uid);
    }
}
