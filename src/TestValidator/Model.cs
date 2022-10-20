using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace TestBlazor;

public class Model
{
	[Required] [MaxLength(6)] public string Name { get; set; } = default!;
}

public class FluentValidatorModel : AbstractValidator<Model>
{
	public FluentValidatorModel()
	{
		
    	RuleFor(x => x.Name).MaximumLength(6).WithMessage("controlled by fluent: too long");
	}
}