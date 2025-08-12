using backend.Core.Dto.GeneralDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Core.Services.Shared
{
    public class ErrorResponse
    {
        public static GeneralServiceResponseDto CreateErrorResponse(int statusCode, string message)
        {
            return new GeneralServiceResponseDto
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
