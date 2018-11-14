using FluentValidation.Results;
using MediatR;

namespace Massena.Infrastructure.Validation
{
    public interface IValidationService
    {
        ValidationResult Validate<T>(T entity) where T : IRequest;
    }
}
