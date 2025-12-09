using backend.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext http)
        {
            try
            {
                await _next(http);
            }
            catch(Exception ex)
            {
                await HandleExceptionAsync(http, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext http, Exception ex)
        {
            _logger.LogError(ex, "Unhandled Exception");

            var (statusCode, message) = MapExceptionToResponse(ex);

            http.Response.ContentType = "application/json";
            http.Response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
            };

            await http.Response.WriteAsJsonAsync(errorResponse);
        }

        private static (int, string) MapExceptionToResponse(Exception ex)
        {
            return ex switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, ex.Message),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ex.Message),
                BadHttpRequestException => (StatusCodes.Status400BadRequest, ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                BudgetException => (StatusCodes.Status400BadRequest, ex.Message),
                ExpenseException => (StatusCodes.Status400BadRequest, ex.Message),
                IncomeException => (StatusCodes.Status400BadRequest, ex.Message),
                AuthException => (StatusCodes.Status400BadRequest, ex.Message),
                ForbiddenException => (StatusCodes.Status403Forbidden, ex.Message),
                AccountGroupException => (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, ex.Message)
            };
        }
    }
}
