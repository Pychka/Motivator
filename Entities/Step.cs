using System;
using System.Collections.Generic;

namespace Motivator.Entities;

public partial class Step
{
    public int Id { get; set; }

    public int GoalId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public virtual Goal Goal { get; set; } = null!;
}
