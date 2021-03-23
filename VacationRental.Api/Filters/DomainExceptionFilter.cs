using System.Net;
using Logistics.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VacationRental.Common;
using VacationRental.Domain;

namespace VacationRental.Api.Filters
{
    public class DomainExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            void SetResult<T>(DomainException<T> exception, HttpStatusCode statusCode) where T : DomainError<T>
            {
                context.Result = new ObjectResult(
                    new ErrorViewModel
                    {
                        ErrorDescription = exception.Error.Description
                    })
                {
                    StatusCode = (int) statusCode
                };
                context.ExceptionHandled = true;
            }

            switch (context.Exception)
            {
                case DomainException<ValidationError> exception:
                    SetResult(exception, HttpStatusCode.BadRequest);
                    break;
                case DomainException<RentalNotFound> exception:
                    SetResult(exception, HttpStatusCode.NotFound);
                    break;
                case DomainException<BookingNotFound> exception:
                    SetResult(exception, HttpStatusCode.NotFound);
                    break;
                case DomainException<RentalIsUnavailable> exception:
                    SetResult(exception, HttpStatusCode.BadRequest);
                    break;
                case DomainException<RentalChangeFailed> exception:
                    SetResult(exception, HttpStatusCode.BadRequest);
                    break;
            }
        }
    }
}