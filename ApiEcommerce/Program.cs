using System.Text;
using ApiEcommerce.Data;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql")));
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();   
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

var secretKey = builder.Configuration.GetValue<String>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{ 
    throw new InvalidOperationException("ApiSettings:SecretKey cannot be null or empty");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // desactivando el https en casos de produccion se pasa  true 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters // definicion de parametros
    {
        ValidateIssuerSigningKey = true, // se debe validar el token con una clave valida
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // el token debe ser utf8
        ValidateIssuer = false, // no se valida el emisor del token
        ValidateAudience = false, // no se valida el publico del token
    };
});


// definicion de los cache profile para mapear parametros 
builder.Services.AddControllers(options => options.CacheProfiles.Add("Default10", new CacheProfile()
{
    Duration = 10
}));


builder.Services.AddControllers(options => options.CacheProfiles.Add("Default20", new CacheProfile()
{
    Duration = 20
}));



builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = true;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "jwt Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header, // estara en la cabecera el header 
        Type = SecuritySchemeType.Http, // schema http
        Scheme =  "bearer", // bearer schema
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    // hacemos referencia al schema
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer"
                },
                // requerido por swagger
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            // lista vacia de scope 
            new List<string>()
        }
    });
    
    c.SwaggerDoc("v1", new OpenApiInfo{ Title =  "ApiEcommerce", Version = "v1" , Description = "API productos y usuarios", TermsOfService = new Uri("https://example.com/terms"), 
        Contact = new OpenApiContact
        {
            Name = "DevTalles",
            Url = new Uri("https://github.com/DevTalles/ApiEcommerce"),
            
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de uso",
            Url = new Uri("https://github.com/DevTalles/ApiEcommerce"),
        }
    });
    
    c.SwaggerDoc("v2", new OpenApiInfo{ Title =  "ApiEcommerce version 2", Version = "v2" , Description = "API productos y usuarios", TermsOfService = new Uri("https://example.com/terms"), 
        Contact = new OpenApiContact
        {
            Name = "DevTalles",
            Url = new Uri("https://github.com/DevTalles/ApiEcommerce"),
            
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de uso",
            Url = new Uri("https://github.com/DevTalles/ApiEcommerce"),
        }
    });
    
});

var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    // options.ApiVersionReader = ApiVersionReader.Combine(new QueryStringApiVersionReader("api-version")); //?api-version
});

//api explorer para que swaggger pueda mostrar las versiones correctamente
apiVersioningBuilder.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // v1,v2,v3
    options.SubstituteApiVersionInUrl = true; // api/v{version}/products
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5243").AllowAnyHeader().AllowAnyMethod();
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
    });
}



app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();