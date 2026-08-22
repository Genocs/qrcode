using System.Net;
using Genocs.WebApi.Exceptions;

namespace Genocs.BarcodeLibrary.WebApi.Exceptions;

public sealed class ExceptionToResponseMapper : IExceptionToResponseMapper
{
    public ExceptionResponse? Map(Exception exception)
        => exception switch
        {
            ArgumentException ex => new ExceptionResponse(
                new { message = ex.Message },
                HttpStatusCode.BadRequest),

            KeyNotFoundException ex => new ExceptionResponse(
                new { message = ex.Message },
                HttpStatusCode.NotFound),

            FileNotFoundException ex => new ExceptionResponse(
                new { message = ex.Message },
                HttpStatusCode.NotFound),

            DirectoryNotFoundException ex => new ExceptionResponse(
                new { message = ex.Message },
                HttpStatusCode.NotFound),

            InvalidOperationException ex => new ExceptionResponse(
                new { message = ex.Message },
                HttpStatusCode.BadRequest),

            _ => new ExceptionResponse(
                new { message = "An unexpected error occurred while generating the QR code." },
                HttpStatusCode.InternalServerError)
        };
}