using ConsoleGame.Db;
using ConsoleGame.Entities;
using ConsoleGame.Events;
using ConsoleTables;
using ConsoleTools;
using Rebus.Bus;
using System;

namespace ConsoleGame
{
	public class Gamer : IGamer
	{
		private readonly IDbManager _dbManager;
		private readonly GameConfig _gameConfig;
		private readonly IPlayGame _playGame;
		private readonly IBus _bus;
		private string[] _args;

		public Gamer(
			IDbManager dbManager
			, IPlayGame playGame
			, GameConfig gameConfig
			, IBus bus)
		{
			_dbManager = dbManager;
			_playGame = playGame;
			_gameConfig = gameConfig;
			_bus = bus;
		}

		public void Start(string[] args)
		{
			_args = args;
			CheckSetup();
			WriteScore();
			WriteMenu();
		}

		private void WriteMenu()
		{
			var menu = new ConsoleMenu(_args, level: 0)
				.Add("Play Easy Game", () => _playGame.Play(GameLevel.Easy))
				.Add("Play Normal Game", () => _playGame.Play(GameLevel.Normal))
				.Add("Play Hard Game", () => _playGame.Play(GameLevel.Hard))
				.Add("Game History", WriteHistory)
				//.Add("Two", () => SomeAction("Two"))
				//.Add("Three", () => SomeAction("Three"))
				//.Add("Change me", (thisMenu) => thisMenu.CurrentItem.Name = "I am changed!")
				//.Add("Close", ConsoleMenu.Close)
				//.Add("Action then Close", (thisMenu) => { SomeAction("Close"); thisMenu.CloseMenu(); })
				.Add("Exit", () =>
				{
					Environment.Exit(0);
				})
				.Configure(config =>
				{
					config.Selector = "--> ";
					config.EnableFilter = true;
					config.Title = "The Console Game";
					config.EnableWriteTitle = true;
					config.EnableBreadcrumb = false;
					config.WriteHeaderAction = WriteScore;
				});

			menu.Show();
		}

		private void WriteScore()
		{
			Console.WriteLine($"Score : {_gameConfig.NumberOfWins} / {_gameConfig.NumberOfPlays}");
		}

		private void WriteHistory()
		{
			Console.Clear();
			Console.WriteLine("Game History");
			Console.WriteLine(new string('-', 30));

			var history = _dbManager.ListOfLastNItem(_gameConfig.UserId, 10);
			var table = new ConsoleTable("Date / Time", "Duration", "Status");
			table.Options.EnableCount = false;

			foreach (var item in history)
			{
				table.AddRow(item.FinishTime, item.FinishTime - item.StartTime, item.IsTheWinner ? "Won" : "Lost");
			}

			table.Write(Format.MarkDown);

			Console.WriteLine("Please enter to the continue.");
			Console.ReadLine();
		}

		private void CheckSetup()
		{
			if (string.IsNullOrWhiteSpace(_gameConfig.UserName))
			{
				GetUserName();
			}
		}

		private void GetUserName()
		{
			Console.WriteLine("Please Enter User Name : ");
			var userName = Console.ReadLine();
			_gameConfig.UserName = userName;
			_bus.Send(new SaveGameConfig { Config = _gameConfig });
			Console.Clear();
		}
	}
}