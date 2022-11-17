using BeSwarm.Validator.validators;

using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;

using Microsoft.AspNetCore.Authorization.Infrastructure;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.Validator;

public class ValidateContext
{

	public readonly LanguageManager lm = new();
	public readonly bool shownameinmessage = true;
	public readonly List<IValidatorAttribute> validatorsattribute = new();

	public IValidator? FluentValidator;
	public Action<ValidationStrategy<object>>? FluentStrategy;

	public CultureInfo? Culture { get; set; }

	public ValidateContext(bool shownameinmessage)
	{
		validatorsattribute = new();
		validatorsattribute.Add(new ValidatorAttributeRequired());
		validatorsattribute.Add(new ValidatorAttributeMaxLength());
		validatorsattribute.Add(new ValidatorAttributeMinLength());
		validatorsattribute.Add(new ValidatorAttributeStringLength());
		validatorsattribute.Add(new ValidatorAttributeRange());
		this.lm = new();
		this.shownameinmessage = shownameinmessage;
	}
	public void AddValidatorAttribute(IValidatorAttribute validator)
	{
		foreach (var item in validatorsattribute)
		{
			if (item.Type == validator.Type) return;
		}
		validatorsattribute.Add(validator);
	}
	public void RemoveValidatorAttribute(IValidatorAttribute validator)
	{
		foreach (var item in validatorsattribute)
		{
			if (item.Type == validator.Type)

			{
				validatorsattribute.Remove(item);
				return;
			}
		}
	}
	public void ClearValidatorAttribute
		()
	{
		validatorsattribute.Clear();
	}


}
