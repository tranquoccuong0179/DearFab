using System;
using System.Collections.Generic;

namespace DearFab_Model.Entity;

public partial class Review
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid AccountId { get; set; }

    public double Rating { get; set; }

    public string Content { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
