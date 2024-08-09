using BlogIdentityApi.Extensions.ServiceCollectionExtensions;
using BlogIdentityApi.Follow.Repositories;
using BlogIdentityApi.Follow.Repositories.Base;
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


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IUserRepository, UserDapperRepository>();
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