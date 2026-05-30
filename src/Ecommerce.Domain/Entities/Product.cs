using Ecommerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Domain.Entities
{
    public class Product : BaseEntity
    {
        private Product()
        {
        }

        public Product(
            string name,
            string? description,
            decimal price,
            int stock,
            Guid categoryId)
        {
            ValidateName(name);
            ValidatePrice(price);
            ValidateStock(stock);

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            Stock = stock;
            CategoryId = categoryId;
        }

        public string Name { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public decimal Price { get; private set; }

        public int Stock { get; private set; }

        public Guid CategoryId { get; private set; }

        public Category? Category { get; private set; }

        public void Update(
            string name,
            string? description,
            decimal price,
            int stock,
            Guid categoryId)
        {
            ValidateName(name);
            ValidatePrice(price);
            ValidateStock(stock);

            Name = name.Trim();
            Description = description?.Trim();
            Price = price;
            Stock = stock;
            CategoryId = categoryId;
        }

        public void DecreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            if (Stock < quantity)
            {
                throw new InvalidOperationException("Not enough stock available.");
            }

            Stock -= quantity;
        }

        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            Stock += quantity;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name is required.", nameof(name));
            }
        }

        private static void ValidatePrice(decimal price)
        {
            if (price <= 0)
            {
                throw new ArgumentException("Product price must be greater than zero.", nameof(price));
            }
        }

        private static void ValidateStock(int stock)
        {
            if (stock < 0)
            {
                throw new ArgumentException("Product stock cannot be negative.", nameof(stock));
            }
        }
    }
}
