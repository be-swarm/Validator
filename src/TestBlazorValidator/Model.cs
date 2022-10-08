using FluentValidation;

using System.ComponentModel.DataAnnotations;

namespace TestBlazorValidator;

public class Model
{
	[Required]
	public string Name { get; set; }
	[Range(1, 99)]
	public int Age { get; set; }
	[MaxLength(5)]
	[MinLength(1)]

	public List<Sport> Sports { get; set; } = new();
}

public class Sport
{
	[Required]
	public string Name { get; set; }
	[Range(1, 5)]
	public int Rate { get; set; }
}

public class FluentValidatorModel : AbstractValidator<Model>
{
	public FluentValidatorModel()
	{
		RuleSet("Name", () =>
		{
			RuleFor(x => x.Name).NotEmpty().WithMessage("controlled by fluent: not empty");
		});
	}
}