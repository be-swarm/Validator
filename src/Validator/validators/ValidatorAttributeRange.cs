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

public class ValidatorAttributeRange : IValidatorAttribute
{
	public Type Type => typeof(RangeAttribute);

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
				validation.ErrorMessage = context.lm.GetString("InclusiveBetween_Simple").Replace("{PropertyName}", name).Replace("{From}", ((RangeAttribute)attribute).Minimum.ToString()).Replace("{To}", ((RangeAttribute)attribute).Maximum.ToString());
				if (context.shownameinmessage == false)
					validation.ErrorMessage = validation.ErrorMessage.Replace($"'{name}'", "");
			}
		}
		return validation;
	}
}
