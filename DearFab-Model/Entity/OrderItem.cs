using System;
using System.Collections.Generic;

namespace DearFab_Model.Entity;

public partial class OrderItem
{
    public Guid Id { get; set; }

    public Guid ProductSizeId { get; set; }

    public Guid OrderId { get; set; }

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ProductSize ProductSize { get; set; } = null!;
}
