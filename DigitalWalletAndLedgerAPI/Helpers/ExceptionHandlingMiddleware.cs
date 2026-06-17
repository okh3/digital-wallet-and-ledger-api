using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using DigitalWalletAndLedgerAPI.Models;
using Microsoft.EntityFrameworkCore; 

namespace DigitalWalletAndLedgerAPI.Helpers;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during the request.");

            context.Response.ContentType = "application/json";

            int statusCode = StatusCodes.Status500InternalServerError;
            string errorMessage = _env.IsDevelopment() ? ex.Message : "An unexpected server error occurred.";

            switch (ex)
            {
                case WalletNotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    errorMessage = ex.Message; 
                    break;
                case InsufficientFundsException:
                case ArgumentException:
                    statusCode = StatusCodes.Status400BadRequest;
                    errorMessage = ex.Message;
                    break;
                case DbUpdateConcurrencyException:
                    statusCode = StatusCodes.Status409Conflict;
                    errorMessage = "The wallet balance was updated by another transaction. Please try again.";
                    break;
            }

            context.Response.StatusCode = statusCode;

            var response = new { error = errorMessage, timestamp = DateTime.UtcNow };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}