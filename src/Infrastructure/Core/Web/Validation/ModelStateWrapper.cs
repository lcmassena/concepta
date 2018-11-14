using Massena.Infrastructure.Validation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

namespace Massena.Infrastructure.Core.Web.Validation
{
    public class ModelStateWrapper : IValidationDictionary
    {
        private ModelStateDictionary _modelState;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        #region IValidationDictionary Members

        public void AddModelError(string errorMessage)
        {
            _modelState.AddModelError("", errorMessage);
        }

        public void AddModelError(ValidationResult validationResults)
        {
            if (validationResults != null && validationResults.Errors.Any())
                foreach (var error in validationResults.Errors)
                    AddModelError(error.PropertyName, error.ErrorMessage);
        }

        public void AddModelError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }

        public bool IsValid
        {
            get { return _modelState.IsValid; }
        }

        public IDictionary<string, string[]> Errors
        {
            get
            {
                var errorList = _modelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                );

                return errorList;
            }
        }

        #endregion
    }
}
