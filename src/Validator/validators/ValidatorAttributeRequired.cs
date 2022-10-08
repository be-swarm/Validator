using FluentValidation.Resources;
using FluentValidation.Results;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace beswarm.validator.validators;
public class ValidatorAttributeRequired : IValidatorAttribute
{
	public Type Type => typeof(RequiredAttribute);

	public ValidationStatus? IsValid(ValidationAttribute attribute, object? value, string name, ValidateContext context, bool islist)
	{
		ValidationStatus? validation = null;
		if (attribute.GetType() == Type)
		{
			if (!attribute.IsValid(value))
			{
				validation = new();
				validation.Obj = value;
				validation.PropertyName = name;
				validation.AttributeType = attribute.GetType().Name;
				validation.ErrorMessage = context.lm.GetString("NotEmptyValidator").Replace("{PropertyName}", name);
				if (context.shownameinmessage == false)
					validation.ErrorMessage = validation.ErrorMessage.Replace($"'{name}'", "");

			}
			
		}
		return validation;
	}
}

