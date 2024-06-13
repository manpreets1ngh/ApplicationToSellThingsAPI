using ApplicationToSellThings.APIs.Areas.Identity.Data;
using ApplicationToSellThings.APIs.Data;
using ApplicationToSellThings.APIs.Services;
using ApplicationToSellThings.APIs.Services.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationToSellThingsAPIsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationToSellThingsAPIsContext") ?? throw new InvalidOperationException("Connection string 'ApplicationToSellThingsAPIsContext' not found.")));

builder.Services.AddDbContext<ApplicationToSellThingsAPIIdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationToSellThingsAPIIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationToSellThingsAPIsContext' not found.")));

builder.Services.AddIdentity<ApplicationToSellThingsAPIsUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationToSellThingsAPIIdentityContext>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICardService, CardService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClientOrigin", builder =>
    {
        builder.WithOrigins("http://localhost:5282") // Replace with your client application's URL
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<IdentityOptions>(options =>
{
    // Default Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["JWT:Secret"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{   
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowClientOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
