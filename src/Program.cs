using BlogIdentityApi.Extensions.ServiceCollectionExtensions;
using BlogIdentityApi.Follow.Repositories;
using BlogIdentityApi.Follow.Repositories.Base;
using BlogIdentityApi.Options;
using BlogIdentityApi.User.Repositories;
using BlogIdentityApi.User.Repositories.Base;

var builder = WebApplication.CreateBuilder(args);

builder.Services.InitAspnetIdentity(builder.Configuration);
builder.Services.InitAuth(builder.Configuration);
builder.Services.InitSwagger();
builder.Services.InitCors();
builder.Services.RegisterDpInjection();
builder.Services.AddValidators();
builder.Services.AddMediatR();
var rabbitMqSection = builder.Configuration.GetSection("RabbitMq");
builder.Services.Configure<RabbitMqOptions>(rabbitMqSection);

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IUserRepository, UserRabbitMqRepository>();
builder.Services.AddTransient<IFollowRepository, FollowEFRepository>();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("BlazorTestPolicy");

app.Run();