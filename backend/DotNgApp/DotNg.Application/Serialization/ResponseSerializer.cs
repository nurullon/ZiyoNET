using DotNg.Application.Models.Errors;
using DotNg.Domain.Common;
using DotNg.Domain.Common.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace DotNg.Application.Serialization;

public class ResponseSerializer(ILogger<ResponseSerializer> logger)
{
    public ObjectResult ToActionResult(object? value)
    {
        return StatusCode((int)HttpStatusCode.OK, new
        {
            SuccessResult = value
        });
    }

    public ObjectResult ToActionResult(Result result)
    {
        if (result.IsSuccess)
            return StatusCode((int)HttpStatusCode.OK, null);

        return SerializeErrorResult(result);
    }

    public ObjectResult ToActionResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return SerializeSuccessResult(result);

        return SerializeErrorResult(result);
    }


    private ObjectResult SerializeSuccessResult<T>(Result<T> result)
    {
        return StatusCode((int)HttpStatusCode.OK, new
        {
            SuccessResult = result.Value,
        });
    }

    private static ObjectResult StatusCode(int statusCode, object? value)
    {
        return new(value)
        {
            StatusCode = statusCode
        };
    }

    private static  ObjectResult InternalServerError()
    {
        return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse(ErrorCodes.InternalServerError, "Internal server error"));
    }

    private ObjectResult SerializeErrorResult(Result result)
    {
        return SerializeError(result.Error);
    }

    private  ObjectResult SerializeError(Error? error)
    {
        switch (error)
        {
            case ValidationError validationError:
                return StatusCode((int)HttpStatusCode.UnprocessableContent,
                    new ErrorResponse(validationError.Code, validationError.Message)
                    {
                        Errors = validationError.Errors
                    });
            case UnauthorizedError:
                return StatusCode((int)HttpStatusCode.Unauthorized, new ErrorResponse(error.Code, error.Message));
            case UserError:
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(error.Code, error.Message));
            case ServiceError:
                return InternalServerError();
            default:
                logger.LogError("Error occurred: {Error}", JsonSerializer.Serialize(error));
                return InternalServerError();
        }
    }
}