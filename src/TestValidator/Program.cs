// See https://aka.ms/new-console-template for more information

using beswarm.validator;

using FluentValidation.Results;


using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using beswarm.validator.validators;
using TestBlazor;

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

        // add fluent validator
        FluentValidatorModel fv = new();
        context.FluentValidator = fv;
        listvalidationfailure = await Validate.ValidateObject(t, context);
        foreach (var item in listvalidationfailure)
        {
            Console.WriteLine($"*** error **** Field: {item.PropertyName} value={item.Obj} attribute:{item.AttributeType} message:{item.ErrorMessage}");
        }




        //      RequiredAttribute? ra=Validate.GetAttributeIfExist<RequiredAttribute>(t, nameof(t.String));
        //var ra2 = Validate.GetAttributeIfExist<RequiredAttribute>(t, nameof(t.String2));

        //var ra3 = Validate.GetAttributeIfExist<MinLengthAttribute>(t, nameof(t.List));

        //var ra43 = Validate.GetAttributeIfExist<MinLengthAttribute>(t, "NotFoound");

        Console.ReadKey();
    }
}
