using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Domain.Entities
{
    public class Category
    {
        private readonly List<Product> _products = [];

        private Category()
        {
            //It's useful to EF Core to create objects to read data.
            //And we can use the constructor overlading to create objects normally without breaking the app.

        }

        public Category(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name is required.", nameof(name));
            }

            Name = name.Trim();
            Description = description?.Trim();
        }

        public string Name { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public void Update(string name, string? description)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Category name is required.", nameof(name));
            }

            Name = name.Trim();
            Description = description?.Trim();
        }
    }
}
