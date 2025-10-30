using System.Collections.Generic;

namespace Firmeza.Core.Entities;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}