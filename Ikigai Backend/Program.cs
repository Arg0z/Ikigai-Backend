using Ikigai_Backend.Constants;
using Ikigai_Backend.Database;
using Ikigai_Backend.DbModels;
using Ikigai_Backend.Services;   // Add this if not present
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Read JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];
var key = jwtSettings["Key"];

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ikigai API", Version = "v1" });

    // Add JWT Bearer definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure Entity Framework and SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IkigaiDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add controllers
builder.Services.AddControllers();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddScoped<AnimeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<IkigaiDbContext>();

    // Check if admin exists
    if (!context.Users.Any(u => u.Email == "admin@ikigai.com"))
    {
        var adminUser = new User
        {
            Name = "Admin",
            Email = "admin@ikigai.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!"),
            Created_at = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new UserRole { RoleName = Roles.Admin }
            }
        };
        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}

app.UseAuthentication();
app.UseAuthorization();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();