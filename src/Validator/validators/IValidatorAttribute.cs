using FluentValidation.Resources;
using FluentValidation.Results;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.Validator.validators;

public interface IValidatorAttribute
{
    Type Type { get; }
    ValidationStatus? IsValid(ValidationAttribute attribute, object? value, string name, ValidateContext context, bool islist = false);
}
