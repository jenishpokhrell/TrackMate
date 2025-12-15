using backend.Dto.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Model.Dto.Auth
{
    public sealed record MeResponseDto
    {
         public string NewToken { get; init; }
         public UserInfo UserInfo { get; init; }
    }
}
