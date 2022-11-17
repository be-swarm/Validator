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
public class ValidatorAttributeMaxLength : IValidatorAttribute
{
	public Type Type => typeof(MaxLengthAttribute);

	public ValidationStatus? IsValid(ValidationAttribute? attribute, object? value, string name, ValidateContext context, bool islist)
	{
		ValidationStatus? validation = null;
		if (attribute?.GetType() == Type)
		{

			if (!attribute.IsValid(value))
			{
				validation = new()
				{
					Obj = value,
					PropertyName = name,
					AttributeType = attribute.GetType().Name
				};
				if (islist == true)
					validation.ErrorMessage = context.lm.GetString("LessThanOrEqualValidator",context.Culture).Replace("{PropertyName}", name).Replace("{ComparisonValue}", ((MaxLengthAttribute)attribute).Length.ToString());
				else
					validation.ErrorMessage = context.lm.GetString("MaximumLength_Simple", context.Culture).Replace("{PropertyName}", name).Replace("{MaxLength}", ((MaxLengthAttribute)attribute).Length.ToString());
				if (context.shownameinmessage == false)
					validation.ErrorMessage = validation.ErrorMessage.Replace($"'{name}'", "");
			}
		}
		return validation;
	}
}
