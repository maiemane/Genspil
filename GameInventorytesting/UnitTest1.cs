using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Xunit;

namespace Genspil.Tests
{
    public class GameInventoryTests : IDisposable
    {
        private const string TestFilePath = "test_games.json";
        private GameInventory inventory;

        public GameInventoryTests()
        {
            // Ændr FilePath i GameInventory til testfilen
            var filePathField = typeof(GameInventory).GetField("FilePath", BindingFlags.Static | BindingFlags.NonPublic);
            filePathField.SetValue(null, TestFilePath);

            // Sørg for, at testfilen starter tom
            if (File.Exists(TestFilePath))
                File.Delete(TestFilePath);
            File.WriteAllText(TestFilePath, "[]");

            // Opret en ny instans af inventory
            inventory = new GameInventory();
        }

        [Fact]
        public void AddGame_ShouldAddNewGame()
        {
            // Arrange
            var game = new Game("Catan", Game.Condition.God, 250, 2, "Strategy", 4, "Original");

            // Act
            inventory.AddGame(game);

            // Assert: Game skal være tilføjet med korrekt navn og stock
            var games = inventory.GetGames();
            Assert.Single(games);
            Assert.Equal("Catan", games[0].Name);
            Assert.Equal(2, games[0].Stock);
        }

        [Fact]
        public void AddGame_ShouldUpdateStockIfGameExists()
        {
            // Arrange: Tilføj samme spil to gange
            var game1 = new Game("Catan", Game.Condition.God, 250, 1, "Strategy", 4, "Original");
            var game2 = new Game("Catan", Game.Condition.God, 250, 2, "Strategy", 4, "Original");

            // Act
            inventory.AddGame(game1);
            inventory.AddGame(game2);

            // Assert: Der skal kun være ét spil med samlet stock på 3
            var games = inventory.GetGames();
            Assert.Single(games);
            Assert.Equal(3, games[0].Stock);
        }

        [Fact]
        public void RemoveGame_ShouldRemoveGameByName()
        {
            // Arrange
            var game = new Game("UNO", Game.Condition.OK, 50, 1, "Party", 4, "Classic");
            inventory.AddGame(game);

            // Act
            inventory.RemoveGame("UNO");

            // Assert: Spillet skal være fjernet
            var games = inventory.GetGames();
            Assert.Empty(games);
        }

        [Fact]
        public void EditGame_ShouldChangeGameName()
        {
            // Arrange
            var game = new Game("Chess", Game.Condition.Slidt, 30, 1, "Classic", 2, "Wood");
            inventory.AddGame(game);

            // Act
            inventory.EditGame("Chess", "Wood", "edit_name", "Skak");

            // Assert: Spillets navn skal være opdateret
            var games = inventory.GetGames();
            Assert.Single(games);
            Assert.Equal("Skak", games[0].Name);
        }

        [Fact]
        public void SearchGame_ShouldReturnCorrectGame()
        {
            // Arrange: Tilføj to spil
            var game1 = new Game("Ticket to Ride", Game.Condition.God, 300, 3, "Strategy", 5, "Europe");
            var game2 = new Game("UNO", Game.Condition.OK, 50, 2, "Party", 4, "Classic");
            inventory.AddGame(game1);
            inventory.AddGame(game2);

            // Act: Søg efter spillet "UNO"
            var results = inventory.SearchGame(name: "UNO");

            // Assert: Kun ét spil skal returneres med navnet "UNO"
            Assert.Single(results);
            Assert.Equal("UNO", results[0].Name);
        }

        public void Dispose()
        {
            // Ryd op efter testkørslen ved at slette testfilen
            if (File.Exists(TestFilePath))
                File.Delete(TestFilePath);
        }
    }
}