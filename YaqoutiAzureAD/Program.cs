using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Yaqouti Azure AD Test",Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Description = "Oauth2.0 uses AuthorizationCode flow",
            Name = "Oauth2",
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri(builder.Configuration["YaqoutiAzureAd:AuthorizationUrl"]),
                    TokenUrl = new Uri(builder.Configuration["YaqoutiAzureAd:TokenUrl"]),
                    Scopes = new Dictionary<string, string>
                    {
                        { builder.Configuration["YaqoutiAzureAd:Scopes"],"Access API as User"}
                    }
                }
            }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme,Id = "oauth2"}
                },
                new [] {builder.Configuration["YaqoutiAzureAd:Scopes"] }
            }
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.OAuthClientId(builder.Configuration["YaqoutiAzureAd:ClientId"]);
            c.OAuthUsePkce();
            c.OAuthScopeSeparator(",");
        });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
