using System;
using System.Collections.Generic;

namespace DearFab_Model.Entity;

public partial class Transaction
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public long OrderCode { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}
