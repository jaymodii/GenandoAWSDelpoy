using Common.Constants;
using Common.Hubs;
using DataAccessLayer.Data;
using GenandoAPI.Extensions;
using GenandoAPI.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//Add Services to the container
builder.Services.AddHttpContextAccessor();

builder.Services.AddRouting(options => options.LowercaseUrls = true); //Lower case url in swagger

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true; //Suppressing default validation of model for API
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Inside ApplicationConfiguration Extension Method
builder.Services.ConnectDatabase(builder.Configuration);
builder.Services.RegisterFluentValidation();
builder.Services.RegisterRepository();
builder.Services.RegisterServices();
builder.Services.ConfigureCors();
builder.Services.SetRequestBodySize();
builder.Services.RegisterAutoMapper();
builder.Services.ConfigAuthentication(builder.Configuration);
builder.Services.RegisterMail(builder.Configuration);
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors(SystemConstants.CorsPolicy);

//Auto migration
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

app.UseAuthentication();


app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<BroadcastHub>("/api/notify");
    endpoints.MapControllers();
});

app.Run();
