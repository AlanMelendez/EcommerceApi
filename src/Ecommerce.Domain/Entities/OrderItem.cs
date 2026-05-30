using Ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        private OrderItem()
        {
        }

        public OrderItem(
            Guid productId,
            string productName,
            decimal unitPrice,
            int quantity)
        {
            if (productId == Guid.Empty)
            {
                throw new ArgumentException("Product id is required.", nameof(productId));
            }

            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ArgumentException("Product name is required.", nameof(productName));
            }

            if (unitPrice <= 0)
            {
                throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));
            }

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            ProductId = productId;
            ProductName = productName.Trim();
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public Guid OrderId { get; private set; }

        public Order? Order { get; private set; }

        public Guid ProductId { get; private set; }

        public Product? Product { get; private set; }

        public string ProductName { get; private set; } = string.Empty;

        public decimal UnitPrice { get; private set; }

        public int Quantity { get; private set; }

        public decimal Subtotal => UnitPrice * Quantity;
    }
}
