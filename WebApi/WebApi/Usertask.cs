using System;
using System.Collections.Generic;

namespace WebApi;

public partial class Usertask
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateOnly? Dtcreate { get; set; }

    public DateOnly? Dtupdate { get; set; }
}
