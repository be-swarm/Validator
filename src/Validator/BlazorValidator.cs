using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BeSwarm.Validator;

public class BlazorValidator : ComponentBase
{
	private static readonly char[] Separators = { '.', '[' };
	[CascadingParameter]
	private EditContext CurrentEditContext { get; set; } = null!;

	[Parameter] public ValidateContext ValidateContext { get; set; } = default!;

	ValidationMessageStore messages = default!;
	protected override void OnInitialized()
	{
		if (CurrentEditContext == null)
		{
			throw new NullReferenceException(nameof(CurrentEditContext));
		}
		if (ValidateContext == null)
		{
			throw new NullReferenceException(nameof(ValidateContext));
		}

		messages = new ValidationMessageStore(CurrentEditContext);
		CurrentEditContext.OnValidationRequested += async (sender, eventArgs) => await ValidateModelAsync();
		CurrentEditContext.OnFieldChanged +=
			async (sender, eventArgs) => await ValidateFieldAsync(eventArgs.FieldIdentifier);

	}
	

	public async Task<bool> ValidateModelAsync()
	{
		messages.Clear();
		
		var result = await Validate.ValidateObject(CurrentEditContext.Model, ValidateContext);
		foreach (var item in result)
		{
			if (item.ErrorMessage != "")
			{
				var fieldIdentifier = ToFieldIdentifier(CurrentEditContext, item.PropertyName);
				messages.Add(fieldIdentifier, item.ErrorMessage);
			}
		}
	
		CurrentEditContext.NotifyValidationStateChanged();
		return result!.Count ==0;
	}
	private async Task ValidateFieldAsync(FieldIdentifier fieldidentifier)
	{
		messages.Clear(fieldidentifier);
		var result = await Validate.ValidateObject(CurrentEditContext.Model, ValidateContext!);
		foreach (var item in result)
		{
			var fieldIdentifier = ToFieldIdentifier(CurrentEditContext, item.PropertyName);
			// only errors for this field
			if (fieldIdentifier.Model == fieldidentifier.Model && fieldIdentifier.FieldName == fieldidentifier.FieldName)
			{
				messages.Add(fieldIdentifier, item.ErrorMessage);
			}
		}
		CurrentEditContext.NotifyValidationStateChanged();
	}
	private static FieldIdentifier ToFieldIdentifier(in EditContext editContext, in string propertyPath)
	{
		// This code is taken from an article by Steve Sanderson (https://blog.stevensanderson.com/2019/09/04/blazor-fluentvalidation/)
		// all credit goes to him for this code.

		// This method parses property paths like 'SomeProp.MyCollection[123].ChildProp'
		// and returns a FieldIdentifier which is an (instance, propName) pair. For example,
		// it would return the pair (SomeProp.MyCollection[123], "ChildProp"). It traverses
		// as far into the propertyPath as it can go until it finds any null instance.

		var obj = editContext.Model;
		var nextTokenEnd = propertyPath.IndexOfAny(Separators);

		// Optimize for a scenario when parsing isn't needed.
		if (nextTokenEnd < 0)
		{
			return new FieldIdentifier(obj, propertyPath);
		}

		ReadOnlySpan<char> propertyPathAsSpan = propertyPath;

		while (true)
		{
			var nextToken = propertyPathAsSpan.Slice(0, nextTokenEnd);
			propertyPathAsSpan = propertyPathAsSpan.Slice(nextTokenEnd + 1);

			object? newObj;
			if (nextToken.EndsWith("]"))
			{
				// It's an indexer
				// This code assumes C# conventions (one indexer named Item with one param)
				nextToken = nextToken.Slice(0, nextToken.Length - 1);
				var prop = obj.GetType().GetProperty("Item");

				if (prop is not null)
				{
					// we've got an Item property
					var indexerType = prop.GetIndexParameters()[0].ParameterType;
					var indexerValue = Convert.ChangeType(nextToken.ToString(), indexerType);

					newObj = prop.GetValue(obj, new[] { indexerValue });
				}
				else
				{
					// If there is no Item property
					// Try to cast the object to array
					if (obj is object[] array)
					{
						var indexerValue = int.Parse(nextToken);
						newObj = array[indexerValue];
					}
					else
					{
						throw new InvalidOperationException($"Could not find indexer on object of type {obj.GetType().FullName}.");
					}
				}
			}
			else
			{
				// It's a regular property
				var prop = obj.GetType().GetProperty(nextToken.ToString());
				if (prop == null)
				{
					throw new InvalidOperationException($"Could not find property named {nextToken.ToString()} on object of type {obj.GetType().FullName}.");
				}
				newObj = prop.GetValue(obj);
			}

			if (newObj == null)
			{
				// This is as far as we can go
				return new FieldIdentifier(obj, nextToken.ToString());
			}

			obj = newObj;

			nextTokenEnd = propertyPathAsSpan.IndexOfAny(Separators);
			if (nextTokenEnd < 0)
			{
				return new FieldIdentifier(obj, propertyPathAsSpan.ToString());
			}
		}
	}

}
