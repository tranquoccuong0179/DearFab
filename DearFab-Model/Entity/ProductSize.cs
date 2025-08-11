using System;
using System.Collections.Generic;

namespace DearFab_Model.Entity;

public partial class ProductSize
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid SizeId { get; set; }

    public double? Price { get; set; }

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual Size Size { get; set; } = null!;
}
