using System.Net;
using System.Text.Json;
using ClearFX.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ClearFX.API.Middleware;

public class ExceptionMiddleware (RequestDelegate next , ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }

      
        
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, title, errors) = ex switch
        {
            ValidationException vex => (
                HttpStatusCode.BadRequest,
                vex.Errors.FirstOrDefault()?.ErrorMessage ?? "Validation failed.",
                vex.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                }).ToArray()
            ),

            DomainException => (
                HttpStatusCode.UnprocessableEntity,
                ex.Message,
                null
            ),

            InvalidOperationException => (
                HttpStatusCode.BadRequest,
                ex.Message,
                null
            ),

            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                ex.Message,
                null
            ),

            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                ex.Message,
                null
            ),

            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                null
            )
        };

        var problem = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Instance = context.Request.Path
        };

        if (errors is not null)
            problem.Extensions["errors"] = errors;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}