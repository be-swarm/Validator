using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Resources;
using FluentValidation.Results;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using static FluentValidation.AssemblyScanner;

namespace BeSwarm.Validator;


public class ValidationStatus
{
	/// <summary>
	/// The full property name
	/// </summary>
	public string PropertyName { get; set; } = "";
	/// <summary>
	/// the error code
	/// </summary>
	public string AttributeType { get; set; } = "";
	/// <summary>
	/// The error message
	/// </summary>
	public string ErrorMessage { get; set; } = "";
	/// <summary>
	/// The object value
	/// </summary>
	public object? Obj { get; set; } = default;
}

public static class Validate
{
	public static async Task<List<ValidationStatus>> ValidateObject(object obj, ValidateContext context )
	{
		var result= ValidateObjectRecursive(obj, context, "");
		if (context.FluentValidator is { })
		{
			ValidationContext<object> fluentcontext;
			if (context.FluentStrategy is { })
			{
				fluentcontext = ValidationContext<object>.CreateWithOptions(obj, context.FluentStrategy);
			}
			else
			{
				fluentcontext = new ValidationContext<object>(obj);
			}
			var validationResults = await context.FluentValidator.ValidateAsync(fluentcontext);
			foreach (var validationResult in validationResults.Errors)
			{
				result.Add(new()
				{
					AttributeType = "Fluent",
					 ErrorMessage = validationResult.ErrorMessage,
					 Obj=null,
					  PropertyName = validationResult.PropertyName
				});
			}
		}
		return result;
	}
	public static T? GetAttributeIfExist<T>(object obj, string Name) where T : ValidationAttribute
	{
		return (T)obj.GetType().GetProperty(Name)?.GetCustomAttribute(typeof(T))!;
	}

	private static List<ValidationStatus> ValidateObjectRecursive(object obj, ValidateContext context, string Prefix = "")
	{
		List<ValidationStatus> result = new();
		Type objType = obj.GetType();
		var properties = obj.GetType().GetProperties().Where(prop => prop.CanRead && prop.GetIndexParameters().Length == 0).ToList();
		if (properties is { })
		{

			foreach (PropertyInfo property in properties)
			{
				string Name = "";
				if (Prefix != "") Name = Prefix + "." + property.Name;
				else Name = property.Name;
				object? value = property.GetValue(obj, null);
				if (property.PropertyType.IsValueType == true || property.PropertyType == typeof(string))
				{
					List<ValidationStatus> errors = ValidateProperty(property, value, Name, context);
					result.AddRange(errors);
				}
				else
				{
					var asEnumerable = value as IEnumerable;
					//  object propery first
					List<ValidationStatus> errors = ValidateProperty(property, value, Name, context, asEnumerable is { } ? true : false);
					result.AddRange(errors);
					if (value == null) continue;

					if (asEnumerable is { })
					{   // recursive on child items 
						int i = 0;
						foreach (var item in asEnumerable)
						{
							if (Prefix != "") Name = string.Format($"{Prefix}.{property.Name}[{i}]");
							else Name = string.Format($"{property.Name}[{i}]");
							result.AddRange(ValidateObjectRecursive(item, context, Name));
							i++;
						}
					}
					else
					{   // recursive object
						if (Prefix != "") Name = string.Format($"{Prefix}.{property.Name}");
						else Name = string.Format($"{property.Name}");
						result.AddRange(ValidateObjectRecursive(value, context, Name));
					}
				}

			}
		}
		return result;
	}
	private static List<ValidationStatus> ValidateProperty(PropertyInfo property, object? value, string Name, ValidateContext context, bool islist = false)
	{
		// source Microsoft DataAnnotations to see ValidationAttribute
		//https://github.com/microsoft/referencesource/tree/master/System.ComponentModel.DataAnnotations/DataAnnotations
		List<ValidationStatus> listvalidationfailure = new();
		var attributes = property.GetCustomAttributes(false);
		foreach (var item in attributes)
		{
			// only these Validators
			var validationattribute = item as ValidationAttribute;
			if (validationattribute is { })
			{
				foreach (var validator in context.validatorsattribute)
				{
					var result = validator.IsValid(validationattribute, value, Name, context, islist);
					if (result is { }) listvalidationfailure.Add(result);
				}
			}
		}
		return listvalidationfailure;
	}
	private static readonly char[] Separators = { '.', '[' };
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