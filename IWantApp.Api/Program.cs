using System.Text;

using IWantApp.Api.Endpoints.Categories;
using IWantApp.Api.Endpoints.Employees;
using IWantApp.Api.Endpoints.Products;
using IWantApp.Api.Endpoints.Security;
using IWantApp.Api.Infra.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
                context.Configuration.GetConnectionString("SqlServer"),
                sinkOptions: new MSSqlServerSinkOptions
                {
                   AutoCreateSqlTable = true,
                   TableName = "LogAPI"
                },
                restrictedToMinimumLevel: LogEventLevel.Warning);
});

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration.GetConnectionString("SqlServer")!);
builder.Services.AddSingleton<SqlConnection>(new SqlConnection(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options => 
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("EmployeePolicy", p => 
            p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));

    options.AddPolicy("Employee999Policy", p => 
            p.RequireAuthenticatedUser().RequireClaim("EmployeeCode", "999"));
});

builder.Services.AddAuthentication(x => 
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtTokenSettings = builder.Configuration.GetSection("JwtBearerTokenSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
       ValidateActor = true,
       ValidateAudience = true,
       ValidateLifetime = true,
       ValidateIssuer = true,
       ValidateIssuerSigningKey = true,
       ValidIssuer = jwtTokenSettings["Issuer"],
       ValidAudience = jwtTokenSettings["Audience"],
       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings["SecretKey"]!)),
       ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddScoped<QueryAllUsersWithClaimName>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/healthcheck");

app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryGetById.Template, CategoryGetById.Methods, CategoryGetById.Handle);
app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);

app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);

app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
app.MapMethods(ProductGetById.Template, ProductGetById.Methods, ProductGetById.Handle);
app.MapMethods(ProductGetShowcase.Template, ProductGetShowcase.Methods, ProductGetShowcase.Handle);

app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    return error switch {
        SqlException => Results.Problem(title: "Database is down", statusCode: 500),
        BadHttpRequestException => Results.Problem(title: "Error in data conversion. See all information sent", statusCode: 500),
        _ => Results.Problem(title: "An error ocurred.", statusCode: 500), 
    };
});

app.Run();