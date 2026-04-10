using Microsoft.AspNetCore.Mvc.Filters;

public class SimpleFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine(" Before API");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine("After API");
    }
}