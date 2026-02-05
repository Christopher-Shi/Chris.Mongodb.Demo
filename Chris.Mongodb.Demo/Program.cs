
using Chris.Mongodb.Demo.Services;

namespace Chris.Mongodb.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // 注册MongoDB上下文（单例模式）
            builder.Services.AddSingleton<MongoDbContext>();
            // 注册用户服务（作用域，适配Web请求生命周期）
            builder.Services.AddScoped<UserService>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
