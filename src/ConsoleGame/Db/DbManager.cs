using ConsoleGame.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleGame.Db
{
  public class DbManager : IDbManager
  {
    private const string dbName = "game.db";

    private SqliteConnection _defaultConnection;

    public DbManager()
    {
      SqlMapper.AddTypeHandler<Guid>(new SqliteGuidTypeHandler());
      CheckDbFile();
      CreateDefaultConnection();
      CreateTablesIfNotExists();
    }

    private void CreateTablesIfNotExists()
    {
      _defaultConnection.Execute(@"CREATE TABLE IF NOT EXISTS GameConfig (
  Id INTEGER PRIMARY KEY,
  UserId TEXT NOT NULL,
  UserName TEXT NOT NULL,
  NumberOfPlays INTEGER NOT NULL, 
  NumberOfWins INTEGER NOT NULL
)");
      _defaultConnection.Execute(@"CREATE TABLE IF NOT EXISTS PlayHistory (
  Id INTEGER PRIMARY KEY,
  UserId TEXT NOT NULL,
  StartTime DATETIME NOT NULL,
  FinishTime DATETIME NOT NULL,
  IsTheWinner TINYINT NOT NULL
)");
    }

    private void CreateDefaultConnection()
    {
      _defaultConnection = GetConnection();
      _defaultConnection.Open();
    }

    public SqliteConnection GetConnection()
    {
      SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder
      {
        DataSource = dbName,
      };

      var con = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);
      
      return con;
    }

    public void CheckDbFile()
    {
      if (!File.Exists(dbName))
      {
        var f = File.OpenWrite(dbName);
        f.Close();
      }
    }

    public GameConfig GetGameConfig()
    {
      var gameConfig = _defaultConnection.QueryFirstOrDefault<GameConfig>(@"
SELECT Id, UserId, UserName, NumberOfPlays, NumberOfWins
FROM GameConfig
WHERE Id = 1");

      if (gameConfig == null)
      {
        gameConfig = new GameConfig
        {
          Id = 1,
          UserId = Guid.NewGuid(),
          UserName = "",
          NumberOfPlays = 0,
          NumberOfWins = 0
        };
        SetGameConfig(gameConfig);
      }

      return gameConfig;
    }

    public List<PlayHistory> ListOfLastNItem(Guid userId, int n)
    {
      var list = _defaultConnection.Query<PlayHistory>(@"
SELECT Id, UserId, StartTime, FinishTime, IsTheWinner
FROM PlayHistory
WHERE UserId = @UserId
ORDER BY StartTime desc
LIMIT @n
", new { UserId = userId, n}
        );

      return list.AsList();
    }

    public void SetGameConfig(GameConfig gameConfig)
    {
      _defaultConnection.Execute(@"
INSERT INTO GameConfig (Id, UserId, UserName, NumberOfPlays, NumberOfWins)
VALUES (@Id, @UserId, @UserName, @NumberOfPlays, @NumberOfWins)
ON CONFLICT(Id) DO UPDATE
SET UserName = excluded.UserName,
    UserId = excluded.UserId,
    NumberOfPlays = excluded.NumberOfPlays,
    NumberOfWins = excluded.NumberOfWins", gameConfig);
    }

    public void SetPlayHistory(PlayHistory history)
    {
      _defaultConnection.Execute(@"
INSERT INTO PlayHistory (UserId, StartTime, FinishTime, IsTheWinner)
VALUES (@UserId, @StartTime, @FinishTime, @IsTheWinner)", history);
    }
  }
}