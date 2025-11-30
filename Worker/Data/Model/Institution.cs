using System;
using System.Collections.Generic;

namespace Worker.Data.Model;

public class Institution
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Customer> Customers { get; set; } = new();
    public string Company { get; set; } = string.Empty;
}
