using FluentValidation.Resources;
using FluentValidation.Results;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.Validator.validators;

public class ValidatorAttributeMinLength : IValidatorAttribute
{
	public Type Type => typeof(MinLengthAttribute);

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
				if (islist == true)
					validation.ErrorMessage = context.lm.GetString("GreaterThanValidator").Replace("{PropertyName}", name).Replace("{ComparisonValue}", ((MinLengthAttribute)attribute).Length.ToString());
				else
					validation.ErrorMessage = context.lm.GetString("MinimumLength_Simple").Replace("{PropertyName}", name).Replace("{MinLength}", ((MinLengthAttribute)attribute).Length.ToString());
				if (context.shownameinmessage == false)
					validation.ErrorMessage = validation.ErrorMessage.Replace($"'{name}'", "");
			}
		}
		return validation;
	}
}

