using System;
using System.Collections.Generic;

namespace DearFab_Model.Entity;

public partial class Size
{
    public Guid Id { get; set; }

    public string Label { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}
