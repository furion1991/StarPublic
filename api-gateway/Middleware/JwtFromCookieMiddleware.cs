namespace ApiGateway.Middleware;

public class JwtFromCookieMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.ContainsKey("AccessToken"))
        {

            var token = context.Request.Cookies["AccessToken"];

            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine("FOUND TOKEN");
                context.Request.Headers.Add("Authorization", $"Bearer {token}");
            }
        }

        await next(context);
    }
}

