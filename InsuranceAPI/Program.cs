
using InsuranceAPI.Context;
using InsuranceAPI.Interfaces;
using InsuranceAPI.Models;
using InsuranceAPI.Repositories;
using InsuranceAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace InsuranceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            #region Context
            builder.Services.AddDbContext<InsuranceManagementContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            #endregion
            #region Repositories
            builder.Services.AddScoped<IRepository<int,Client>,ClientRepository>();
            builder.Services.AddScoped<IRepository<string,User>,UserRepository>();
            #endregion
            #region Services
            builder.Services.AddScoped<IClientService,ClientService>();
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
