using Auth.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new Auth.ApiConvertes.StringConverter())); //Remove white space at json in controllers

// Chama os métodos de configuração modularizados
builder.Services.AddCorsPolicy();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddSwaggerService();
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddGoogleAuthentication(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Api v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
