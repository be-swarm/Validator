# Validator

[![NuGet](https://img.shields.io/nuget/v/beswarm.Validator.svg)](https://www.nuget.org/packages/beswarm.Validator/)
[![NuGet](https://img.shields.io/nuget/dt/beswarm.Validator.svg)](https://www.nuget.org/packages/beswarm.Validator/)


Perform recursive localized DataAnnotations Attribute validation on your models and fluent validations.

Provide BlazorValidator for Blazor apps.

### DataAnnotation Attribute validated
- [Required]
- [Range]
- [MaxLength]
- [MinLength]
- [StringLength]
  



## Sample usage
```csharp
public class Model
{
    [Required]
    [MaxLength(6)]
    public string Name { get; set; }
}

class Program
{


    public static async Task Main(string[] args)
    {

        Model t= new();
        ValidateContext context = new ValidateContext(false);
        var listvalidationfailure = await Validate.ValidateObject(t, context);

        foreach (var item in listvalidationfailure)
        {
           Console.WriteLine($"*** error **** Field: {item.PropertyName} value={item.Obj} attribute:{item.AttributeType} message:{item.ErrorMessage}");
        }
        t.Name = "11";
        listvalidationfailure = await Validate.ValidateObject(t, context);
        foreach (var item in listvalidationfailure)
        {
            Console.WriteLine($"*** error **** Field: {item.PropertyName} value={item.Obj} attribute:{item.AttributeType} message:{item.ErrorMessage}");
        }
        t.Name = "LPLOKUAAA";
        listvalidationfailure = await Validate.ValidateObject(t, context);
        foreach (var item in listvalidationfailure)
        {
            Console.WriteLine($"*** error **** Field: {item.PropertyName} value={item.Obj} attribute:{item.AttributeType} message:{item.ErrorMessage}");
        }
		Console.ReadKey();
    }
}

```
Get attribute on property if exist

ex: check if [Required] is present on a property
```csharp
   RequiredAttribute? ra=Validate.GetAttributeIfExist<RequiredAttribute>(t, nameof(t.Name));
```
      
```csharp
## Using with blazor
```csharp
<EditForm Model="@_model" OnValidSubmit="@SubmitValidForm">
	<beswarm.validator.BlazorValidator @ref="_BlazorValidationValidator" ValidateContext="mycontext" />
```
## Add Fluent validation
Create au fluent class
```csharp
 public class FluentValidatorModel : AbstractValidator<Model>
 {
	public FluentValidatorModel()
	{
			RuleFor(x => x.Name).NotEmpty().WithMessage("controlled by fluent: not empty");
	}
 }
```
```csharp
 // add fluent validator
 FluentValidatorModel fv = new();
 context.FluentValidator = fv;
 listvalidationfailure = await Validate.ValidateObject(t, context);
 foreach (var item in listvalidationfailure)
 {
   Console.WriteLine($"*** error **** Field: {item.PropertyName} value={item.Obj} attribute:{item.AttributeType} message:{item.ErrorMessage}");
 }
```
you can also add a Fluent validation strategy
```csharp
public class FluentValidatorModel : AbstractValidator<Model>
{
	public FluentValidatorModel()
	{
		RuleSet("FilterName", () =>
		{
			RuleFor(x => x.Name).NotEmpty().WithMessage("controlled by fluent: not empty");
		});
	}
}
Action<ValidationStrategy<object>> strategy = new(options => options.IncludeRuleSets("FilterName"));
context.FluentStrategy = strategy;
```

## Company
Be Swarm https://beswarm.fr/developpeur_en/

## Author
thierry roustan


## License
MIT

    
## Versions
- 1.0.0
  - Initial release


 
 ## Documentation
