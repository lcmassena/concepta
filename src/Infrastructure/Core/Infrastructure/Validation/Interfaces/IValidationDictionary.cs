using FluentValidation.Results;
using System.Collections.Generic;

namespace Massena.Infrastructure.Validation
{
    public interface IValidationDictionary
    {
        void AddModelError(string errorMessage);

        void AddModelError(string key, string errorMessage);

        void AddModelError(ValidationResult validationResults);

        bool IsValid { get; }

        IDictionary<string, string[]> Errors { get; }
    }
}
