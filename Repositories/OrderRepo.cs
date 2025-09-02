using E_CommerceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace E_CommerceSystem.Repositories
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ApplicationDbContext _context;

        public OrderRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddOrder(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error (AddOrder): {ex.Message}");
            }
        }

        public void DeleteOrder(int oid)
        {
            try
            {
                var order = _context.Orders.FirstOrDefault(o => o.OID == oid);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error (DeleteOrder): {ex.Message}");
            }
        }

        public IEnumerable<Order> GetAllOrders()
        {
            try
            {
                return _context.Orders.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error (GetAllOrders): {ex.Message}");
            }
        }

        public Order GetOrderById(int oid)
        {
            try
            {
                return _context.Orders.FirstOrDefault(o => o.OID == oid);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error (GetOrderById): {ex.Message}");
            }
        }

        public void UpdateOrder(Order order)
        {
            try
            {
                _context.Orders.Update(order);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException("This product was updated by another user. Please reload and try again.");
            }
        }






        public IEnumerable<Order> GetOrderByUserId(int uid)
        {
            try
            {
                return _context.Orders.Where(o => o.UID == uid).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Database error (GetOrderByUserId): {ex.Message}");
            }
        }
    }
}
