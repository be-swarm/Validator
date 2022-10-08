// See https://aka.ms/new-console-template for more information

using beswarm.validator;

using FluentValidation.Results;


using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using beswarm.validator.validators;
public class SubType
{
    [MaxLength(6)]
    public string SubTypeString { get; set; }
}
public class SubType2
{
    [MaxLength(6)]
    public string SubType2String { get; set; }
}
public class SubType3
{

    public string Moi { get; set; }
    public List<string> Liste { get; set; }


}
public class Types
{
    public int Integer { get; set; }
    public float Float { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(12)]
    [StringLength(14, MinimumLength = 5)]
    public string String { get; set; }
    [StringLength(1)]
    public string String2 { get; set; }
    [Required]
    public string String3 { get; set; }
    [Range(10, 20)]
    public Decimal Decimal { get; set; }
   
    public DateTime DateTime { get; set; }

    public Dictionary<string, SubType> Dictionary { get; set; } = new();

    [Required]
    [MinLength(12)]
    [MaxLength(2)]

    public List<SubType2> List { get; set; } = new();
    [Required]
    [MinLength(2)]
    [MaxLength(12)]
    public List<SubType2> List2 { get; set; }

    public SubType3 Embeeded { get; set; } = new();

}

class Program
{


    public static async Task Main(string[] args)
    {

        Types t = new();
        t.String = "AAAA";
        t.String2 = "AAA";
        t.Dictionary.Add("1", new() { SubTypeString = "KLPOL" });
        t.List.Add(new() { SubType2String = "ITEM list" });
        t.List.Add(new() { SubType2String = "ITEM list" });
        t.List.Add(new() { SubType2String = "ITEM list" });
        ValidateContext context = new ValidateContext(false);
        var listvalidationfailure = await Validate.ValidateObject(t, context);
        foreach (var item in listvalidationfailure)
        {
          
           Console.WriteLine($"*** error **** Field: {item.PropertyName} value={item.Obj} attribute:{item.AttributeType} message:{item.ErrorMessage}");
         
        }
		RequiredAttribute? ra=Validate.GetAttributeIfExist<RequiredAttribute>(t, nameof(t.String));
		var ra2 = Validate.GetAttributeIfExist<RequiredAttribute>(t, nameof(t.String2));
       
		var ra3 = Validate.GetAttributeIfExist<MinLengthAttribute>(t, nameof(t.List));

		var ra43 = Validate.GetAttributeIfExist<MinLengthAttribute>(t, "NotFoound");

		Console.ReadKey();
    }
}
