using Microsoft.AspNetCore.Mvc;

namespace ProductManagement.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class ValidateModelAttribute : TypeFilterAttribute
{
    public ValidateModelAttribute() : base(typeof(ValidateModelFilter))
    { }
}
