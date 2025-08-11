using System;
using System.Collections.Generic;

namespace DearFab_Model.Entity;

public partial class Order
{
    public Guid Id { get; set; }

    public double TotalPrice { get; set; }

    public string? Address { get; set; }

    public string Status { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public Guid? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
