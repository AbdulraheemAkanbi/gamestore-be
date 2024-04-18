using GameStore.Data;
using GameStore.Dtos;
using GameStore.Entities;
using GameStore.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Endpoints
{
    public static class GamesEndpoints
    {
        const string getGamesEndpoint = "getGames";
        public static  RouteGroupBuilder MapGameEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("games")
                .WithParameterValidation();

            //api to GET list of all games
            group.MapGet("/", async (GameStoreContext dbContext) => 
                  await dbContext.Games
                            .Include(game => game.Genre)
                           .Select(game => game.ToGameSummaryDto())
                           .AsNoTracking()
                           .ToListAsync());


            //api to GET games with the respective id
            group.MapGet("/{id}",async (int id, GameStoreContext dbContext) =>
            {
                Game? game = await dbContext.Games.FindAsync(id);
                return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
            })
            .WithName(getGamesEndpoint);

            //api to POST a new game 
            group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                /*  
                 *  user input validation 
                if (string.IsNullOrEmpty(newGame.Name)) {
                    return Results.BadRequest("name is required");
                }*/

                Game game = newGame.ToEntity();

               // game.Genre = dbContext.Genres.Find(newGame.GenreId);

                
                dbContext.Games.Add(game);
               await dbContext.SaveChangesAsync();

                return Results.CreatedAtRoute(getGamesEndpoint, new { id = game.Id }, game.ToGameDetailsDto());
            });
                
            //api to UPDATE a game by the id (PUT)
            group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                //finding the game index by the Id
                var existingGame = await dbContext.Games.FindAsync(id);

                if (existingGame is null)
                {
                    return Results.NotFound();
                }
                dbContext.Entry(existingGame)
                 .CurrentValues
                 .SetValues(updatedGame.ToEntity(id));
                dbContext.SaveChanges();
                return Results.NoContent();
            });

            //api to DELETE games from our List by id
            group.MapDelete("games/{id}", async (int id, GameStoreContext dbContext) =>

            {
               await dbContext.Games
                        .Where(game => game.Id == id)
                        .ExecuteDeleteAsync();

                return Results.NoContent();

            });
            return group;

        }
    }
}
