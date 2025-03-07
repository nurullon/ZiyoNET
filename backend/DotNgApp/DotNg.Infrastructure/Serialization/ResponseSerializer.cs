using DotNg.Domain.Common;
using DotNg.Domain.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DotNg.Infrastructure.Serialization;

public class ResponseSerializer(ILogger<ResponseSerializer> logger)
{
    public ObjectResult Serialize(Result result)
    {
        return result.IsSuccess
            ? CreateSuccessResponse(null)
            : CreateErrorResponse(result.Error);
    }

    public ObjectResult Serialize<T>(Result<T> result)
    {
        return result.IsSuccess
            ? CreateSuccessResponse(result.Value)
            : CreateErrorResponse(result.Error);
    }

    public static ObjectResult SerializeSuccess(object? value)
    {
        return CreateSuccessResponse(value);
    }

    private static ObjectResult CreateSuccessResponse(object? value)
    {
        return new ObjectResult(new { success = true, data = value })
        {
            StatusCode = (int)HttpStatusCode.OK
        };
    }

    private ObjectResult CreateErrorResponse(Error? error)
    {
        if (error == null)
        {
            logger.LogError("An unknown error occurred.");
            return CreateInternalServerError();
        }

        logger.LogError("Error occurred: {Error}", JsonSerializer.Serialize(error));


        switch (error)
        {
            case UnauthorizedError:
                return CreateErrorResponse(HttpStatusCode.Unauthorized, error);
            case UserError:

                return CreateErrorResponse(HttpStatusCode.BadRequest, error);
            case ServiceError:
                return CreateInternalServerError();
            default:
                return CreateInternalServerError();
        }
    }

    private static ObjectResult CreateErrorResponse(HttpStatusCode statusCode, Error error)
    {
        return new ObjectResult(new { success = false, error = new { error.Code, error.Message } })
        {
            StatusCode = (int)statusCode
        };
    }

    private static ObjectResult CreateInternalServerError()
    {
        return new ObjectResult(new { success = false, error = new { code = ErrorCodes.InternalServerError, message = "Internal server error" } })
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}