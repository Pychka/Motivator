using System;
using System.Collections.Generic;

namespace Motivator.Entities;

public partial class Token
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Key { get; set; } = null!;

    public bool IsUsed { get; set; }

    public virtual User User { get; set; } = null!;
}
