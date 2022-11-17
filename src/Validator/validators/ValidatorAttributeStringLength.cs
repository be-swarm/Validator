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
public class ValidatorAttributeStringLength : IValidatorAttribute
{
	public Type Type => typeof(StringLengthAttribute);

	public ValidationStatus? IsValid(ValidationAttribute attribute, object? value, string name, ValidateContext context, bool islist)
	{
		ValidationStatus? validation = null;
		if (attribute.GetType()==Type)
		{
			if (!attribute.IsValid(value))
			{
				validation = new();
				validation.Obj = value;
				validation.PropertyName = name;
				validation.AttributeType = attribute.GetType().Name;
				if (((StringLengthAttribute)attribute).MinimumLength != 0)
					validation.ErrorMessage = context.lm.GetString("Length_Simple", context.Culture).Replace("{PropertyName}", name).Replace("{MinLength}", ((StringLengthAttribute)attribute).MinimumLength.ToString()).Replace("{MaxLength}", ((StringLengthAttribute)attribute).MaximumLength.ToString());
				else
					validation.ErrorMessage = context.lm.GetString("MaximumLength_Simple", context.Culture).Replace("{PropertyName}", name).Replace("{MaxLength}", ((StringLengthAttribute)attribute).MaximumLength.ToString());
				if (context.shownameinmessage == false)
					validation.ErrorMessage = validation.ErrorMessage.Replace($"'{name}'", "");
			}
		}
		return validation;
	}
}
