using Ecommerce.Domain.Common;
using Ecommerce.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Domain.Entities
{
    public class Order : BaseEntity
    {
        private readonly List<OrderItem> _items = [];

        private Order()
        {
        }

        public Order(Guid customerId)
        {
            if (customerId == Guid.Empty)
            {
                throw new ArgumentException("Customer id is required.", nameof(customerId));
            }

            CustomerId = customerId;
            Status = OrderStatus.Pending;
        }

        public Guid CustomerId { get; private set; }

        public User? Customer { get; private set; }

        public OrderStatus Status { get; private set; }

        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal Total => _items.Sum(item => item.Subtotal);

        public void AddItem(
            Guid productId,
            string productName,
            decimal unitPrice,
            int quantity)
        {
            var existingItem = _items.FirstOrDefault(item => item.ProductId == productId);

            if (existingItem is not null)
            {
                throw new InvalidOperationException("Product already exists in this order.");
            }

            var item = new OrderItem(productId, productName, unitPrice, quantity);

            _items.Add(item);
        }

        public void MarkAsPaid()
        {
            if (!_items.Any())
            {
                throw new InvalidOperationException("Order must have at least one item before payment.");
            }

            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Only pending orders can be paid.");
            }

            Status = OrderStatus.Paid;
        }

        public void MarkAsShipped()
        {
            if (Status != OrderStatus.Paid)
            {
                throw new InvalidOperationException("Only paid orders can be shipped.");
            }

            Status = OrderStatus.Shipped;
        }

        public void MarkAsCompleted()
        {
            if (Status != OrderStatus.Shipped)
            {
                throw new InvalidOperationException("Only shipped orders can be completed.");
            }

            Status = OrderStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("Completed orders cannot be cancelled.");
            }

            if (Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Order is already cancelled.");
            }

            Status = OrderStatus.Cancelled;
        }
    }
}
                                                                                                                                                                            