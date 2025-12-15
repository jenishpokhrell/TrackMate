using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Auth
{
    public sealed record MeDto
    {
        public string Token { get; init; }
    }
}
