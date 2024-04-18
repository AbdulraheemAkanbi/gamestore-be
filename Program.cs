using GameStore.Data;
using GameStore.Dtos;
using GameStore.Endpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connString = builder.Configuration.GetConnectionString("GameStore");
            builder.Services.AddSqlite<GameStoreContext>(connString);   
            var app = builder.Build();
            app.MapGameEndpoints();
            app.MapGenresEndpoint();    
            await app.MigrateDbAsync();    

            app.Run();
        }
    }
}
  