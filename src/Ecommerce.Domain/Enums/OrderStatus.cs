using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Domain.Enums
{
    public enum  OrderStatus
    {
        Pending = 1,
        Paid = 2,
        Shipped = 3,
        Completed = 4,
        Cancelled = 5
    }
}
