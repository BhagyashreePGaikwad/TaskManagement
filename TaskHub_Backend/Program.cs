using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using taskHub.Context;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using taskHub.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configure your services
builder.Services.AddDbContext<TestContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connect")));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
var allowedOrigin = builder.Configuration.GetSection("AllowedHosts").Get<string[]>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy(Constant.CorsPolicy, policy =>
    {
        policy.WithOrigins(allowedOrigin).AllowAnyMethod().AllowAnyHeader();
    });
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "DemoJWT", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authentication",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference =new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))

    };
});

var app = builder.Build();
app.Use(async (context, next) => {
    context.Response.Headers.Add("Access-Control-Allow-Origin",allowedOrigin);

    await next();
});

// Configure CORS
app.UseCors(Constant.CorsPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
