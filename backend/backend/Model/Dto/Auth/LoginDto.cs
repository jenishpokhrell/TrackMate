using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Auth
{
    public sealed record LoginDto
    {
       public string Email { get; init; }
       public string Password { get; init; }
    };
}
