namespace DesignPattern.BehaviourPattern.Cor.Solution;

public class AuthRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string IpAddress { get; set; }
}

// Handler kiểm tra username/password
public class AuthenticationHandler : Handler<AuthRequest>
{
    public override HandlerResult Process(Request<AuthRequest> request)
    {
        Console.WriteLine($"[{Name}] Checking credentials...");

        // Giả lập kiểm tra authentication
        if (string.IsNullOrEmpty(request.Data.Username) ||
            string.IsNullOrEmpty(request.Data.Password))
        {
            request.SetResult("Authentication failed: Missing credentials");
            return HandlerResult.Handled;
        }

        if (request.Data.Password != "secret123")
        {
            request.SetResult("Authentication failed: Invalid password");
            return HandlerResult.Handled;
        }

        Console.WriteLine($"[{Name}] ✓ Authentication successful");
        return HandlerResult.Continue;
    }
}

// Handler kiểm tra quyền
public class AuthorizationHandler : Handler<AuthRequest>
{
    private readonly string[] _allowedRoles;

    public AuthorizationHandler(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public override HandlerResult Process(Request<AuthRequest> request)
    {
        Console.WriteLine($"[{Name}] Checking authorization...");

        if (!_allowedRoles.Contains(request.Data.Role))
        {
            request.SetResult($"Authorization failed: Role '{request.Data.Role}' not allowed");
            return HandlerResult.Handled;
        }

        Console.WriteLine($"[{Name}] ✓ Authorization successful");
        return HandlerResult.Continue;
    }
}

// Handler kiểm tra IP whitelist
public class IpWhitelistHandler : Handler<AuthRequest>
{
    private readonly HashSet<string> _allowedIps;

    public IpWhitelistHandler(params string[] allowedIps)
    {
        _allowedIps = new HashSet<string>(allowedIps);
    }

    protected override bool CanHandle(Request<AuthRequest> request)
    {
        // Chỉ check IP nếu role là Admin
        return request.Data.Role == "Admin";
    }

    public override HandlerResult Process(Request<AuthRequest> request)
    {
        Console.WriteLine($"[{Name}] Checking IP whitelist for Admin...");

        if (!_allowedIps.Contains(request.Data.IpAddress))
        {
            request.SetResult($"Access denied: IP '{request.Data.IpAddress}' not whitelisted");
            return HandlerResult.Handled;
        }

        Console.WriteLine($"[{Name}] ✓ IP check passed");
        return HandlerResult.Continue;
    }
}

public class LoggingHandler : Handler<AuthRequest>
{
    public override HandlerResult Process(Request<AuthRequest> request)
    {
        Console.WriteLine($"[{Name}] Logging successful access...");
        Console.WriteLine($"User: {request.Data.Username}, Role: {request.Data.Role}, IP: {request.Data.IpAddress}");

        request.SetResult("Access granted");
        return HandlerResult.Handled;
    }
}
