using ConsoleGame.Entities;
using ConsoleGame.Events;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleGame.Db
{
	public class DbManager : IDbManager, IHandleMessages<SaveGameConfig>, IHandleMessages<SavePlayHistory>
	{
		private const string dbName = "game.db";

		private SqliteConnection _defaultConnection;
		private readonly ILogger _logger;

		public DbManager(
			IEnumerable<SqlMapper.ITypeHandler> typeHandlers
			, ILogger<DbManager> logger)
		{
			_logger = logger;
			foreach (var item in typeHandlers)
			{
				SqlMapper.AddTypeHandler(item.GetType().BaseType.GetGenericArguments()[0], item);
			}
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
  NumberOfWins INTEGER NOT NULL,
  NumberOfEasyPlays INTEGER NOT NULL,
  NumberOfEasyWins INTEGER NOT NULL,
  NumberOfNormalPlays INTEGER NOT NULL,
  NumberOfNormalWins INTEGER NOT NULL,
  NumberOfHardPlays INTEGER NOT NULL,
  NumberOfHardWins INTEGER NOT NULL,
  LastPlayDateTime DATETIME NOT NULL,
  UserPoints INTEGER NOT NULL,
  PointMultiplier REAL NOT NULL
)");
			_defaultConnection.Execute(@"CREATE TABLE IF NOT EXISTS PlayHistory (
  Id INTEGER PRIMARY KEY,
  RowGuid TEXT NOT NULL UNIQUE,
  UserId TEXT NOT NULL,
  Status TEXT NOT NULL,
  Level TEXT NOT NULL,
  StartTime DATETIME NOT NULL,
  FinishTime DATETIME NULL,
  IsTheWinner TINYINT NULL,
  PcNumber INTEGER NOT NULL,
  GamePoint INTEGER NULL,
  IterationNumber INTEGER NOT NULL,
  Shoots TEXT NOT NULL
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
SELECT *
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
					NumberOfWins = 0,
					NumberOfEasyPlays = 0,
					NumberOfEasyWins = 0,
					NumberOfHardPlays = 0,
					NumberOfHardWins = 0,
					NumberOfNormalPlays = 0,
					NumberOfNormalWins = 0,
					LastPlayDateTime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime,
					UserPoints = 0,
					PointMultiplier = 1.0
				};
				SetGameConfigAsync(gameConfig);
			}

			return gameConfig;
		}

		public IEnumerable<PlayHistory> ListOfLastNItem(Guid userId, int n)
		{
			var list = _defaultConnection.Query<PlayHistory>(@"
SELECT *
FROM PlayHistory
WHERE UserId = @UserId and Status = @Status
ORDER BY StartTime desc
LIMIT @n
", new { UserId = userId, n, Status = GameStatus.Finished }
				);

			return list;
		}

		private Task<int> SetGameConfigAsync(object gameConfig)
		{
			return _defaultConnection.ExecuteAsync(@"
INSERT INTO GameConfig (Id, UserId, UserName, NumberOfPlays, NumberOfWins, NumberOfEasyPlays, NumberOfEasyWins, NumberOfNormalPlays, NumberOfNormalWins, NumberOfHardPlays, NumberOfHardWins, LastPlayDateTime, UserPoints, PointMultiplier)
VALUES (@Id, @UserId, @UserName, @NumberOfPlays, @NumberOfWins, @NumberOfEasyPlays, @NumberOfEasyWins, @NumberOfNormalPlays, @NumberOfNormalWins, @NumberOfHardPlays, @NumberOfHardWins, @LastPlayDateTime, @UserPoints, @PointMultiplier)
ON CONFLICT(Id) DO UPDATE
SET UserName = excluded.UserName,
    UserId = excluded.UserId,
    NumberOfPlays = excluded.NumberOfPlays,
    NumberOfWins = excluded.NumberOfWins,
    NumberOfEasyPlays = excluded.NumberOfEasyPlays,
    NumberOfEasyWins = excluded.NumberOfEasyWins,
    NumberOfNormalPlays = excluded.NumberOfNormalPlays,
    NumberOfNormalWins = excluded.NumberOfNormalWins,
    NumberOfHardPlays = excluded.NumberOfHardPlays,
    NumberOfHardWins = excluded.NumberOfHardWins,
    LastPlayDateTime = excluded.LastPlayDateTime,
    UserPoints = excluded.UserPoints,
    PointMultiplier = excluded.PointMultiplier
", gameConfig);
		}

		private Task<int> SetPlayHistoryAsync(object history)
		{
			return _defaultConnection.ExecuteAsync(@"
INSERT INTO PlayHistory (RowGuid, UserId, Status, Level, StartTime, FinishTime, IsTheWinner, PcNumber, IterationNumber, Shoots, GamePoint)
VALUES (@RowGuid, @UserId, @Status, @Level, @StartTime, @FinishTime, @IsTheWinner, @PcNumber, @IterationNumber, @Shoots, @GamePoint)
ON CONFLICT(RowGuid) DO UPDATE
SET UserId = excluded.UserId,
    Status = excluded.Status,
    Level = excluded.Level,
    StartTime = excluded.StartTime,
    FinishTime = excluded.FinishTime,
    IsTheWinner = excluded.IsTheWinner,
    PcNumber = excluded.PcNumber,
    IterationNumber = excluded.IterationNumber,
    Shoots = excluded.Shoots,
    GamePoint = excluded.GamePoint", history);
		}

		public async Task Handle(SaveGameConfig message)
		{
			_logger.LogInformation("{@ClassName}.{@MethodName}({@EventName}) {@Status}.", GetType().FullName, nameof(Handle), nameof(SaveGameConfig), "Starting");
			
			await SetGameConfigAsync(message);
			
			_logger.LogInformation("{@ClassName}.{@MethodName}({@EventName}) {@Status}.", GetType().FullName, nameof(Handle), nameof(SaveGameConfig), "Finished");
		}

		public async Task Handle(SavePlayHistory message)
		{
			_logger.LogInformation("{@ClassName}.{@MethodName}({@EventName}) {@Status}.", GetType().FullName, nameof(Handle), nameof(SavePlayHistory), "Starting");

			await SetPlayHistoryAsync(message);
			
			_logger.LogInformation("{@ClassName}.{@MethodName}({@EventName}) {@Status}.", GetType().FullName, nameof(Handle), nameof(SavePlayHistory), "Finished");
		}
	}
}