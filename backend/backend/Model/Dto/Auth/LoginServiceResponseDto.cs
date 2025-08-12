using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dto.Auth
{
    public class LoginServiceResponseDto
    {
        public string NewToken { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
