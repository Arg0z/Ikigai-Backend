using System.Collections.Generic;

public class SignupRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<string>? Roles { get; set; } // Optional, default to ["User"]
}