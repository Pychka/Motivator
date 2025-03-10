using System;
using System.Collections.Generic;

namespace Motivator.Entities;

public partial class Goal
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public virtual ICollection<Step> Steps { get; set; } = new List<Step>();

    public virtual User User { get; set; } = null!;
}
