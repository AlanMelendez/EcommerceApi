using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid(); //For the id.

        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; protected set; }

        public string? CreatedBy { get; protected set; }

        public string? UpdatedBy { get; protected set; }

        public bool IsDeleted { get; protected set; }

        public DateTime? DeletedAt { get; protected set; }

        public string? DeletedBy { get; protected set; }

        public void SetCreatedBy(string? userId)
        {
            CreatedBy = userId;
        }

        public void SetUpdatedBy(string? userId)
        {
            UpdatedBy = userId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SoftDelete(string? userId)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = userId;
        }
    }
}
