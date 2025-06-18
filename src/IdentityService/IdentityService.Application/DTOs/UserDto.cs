using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.DTOs;
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
}