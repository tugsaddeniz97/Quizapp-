using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Controllers;
using QuizApp.Data;
using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Tests.ControllerTests
{
    public class PlayerControllerTests
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public void PlayerController_GetPlayers_ReturnsEmptyListWhenNoPlayers()
        {
            //Arrange
            var dbContext = GetDbContext();
            //Act
            var result = new PlayerController(dbContext).GetPlayers() as OkObjectResult;
            //Assert
            Assert.NotNull(result);
            var players = result.Value as List<Player>;
            Assert.NotNull(players);
            Assert.Empty(players);

        }

        [Fact]
        public void PlayerController_GetPlayers_ReturnsListOfPlayers()
        {
            //Arrange
            var context = GetDbContext();
            var player1 = new Player
            {
                Username = "Player1",
                CreatedAt = DateTime.Now,
                TotalScore = 0
            };
            var player2 = new Player
            {
          
                Username = "Player2",
                CreatedAt = DateTime.Now,
                TotalScore = 0
            };
            context.Players.AddRange(player1, player2);
            context.SaveChanges();

            var controller = new PlayerController(context);
            var expectedPlayers = new List<Player> { player1, player2 };
            //Act
            var result = controller.GetPlayers() as OkObjectResult;
            //Assert

            Assert.NotNull(result);
            var listOfPlayers = result.Value as List<Player>;
            Assert.NotNull(listOfPlayers);
            Assert.Equal(expectedPlayers.Count, listOfPlayers.Count);
            Assert.Equal(expectedPlayers, listOfPlayers);
        }
    }
}
