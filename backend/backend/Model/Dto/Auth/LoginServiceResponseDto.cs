using backend.Model.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Auth
{
    public sealed record LoginServiceResponseDto
    {
        public string NewToken { get; init; }
        public LoginInfo LoginInfo { get; init; }
    };
}
