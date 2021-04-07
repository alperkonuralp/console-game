using AutoMapper;
using ConsoleGame.Db;
using ConsoleGame.Entities;
using ConsoleGame.Events;
using ConsoleTables;
using ConsoleTools;
using Rebus.Bus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleGame
{
	public class Gamer : IGamer
	{
		private readonly IDbManager _dbManager;
		private readonly GameConfig _gameConfig;
		private readonly IPlayGame _playGame;
		private readonly IBus _bus;
		private readonly IMapper _mapper;
		private string[] _args;

		public Gamer(
			IDbManager dbManager
			, IPlayGame playGame
			, GameConfig gameConfig
			, IBus bus
			, IMapper mapper)
		{
			_dbManager = dbManager;
			_playGame = playGame;
			_gameConfig = gameConfig;
			_bus = bus;
			_mapper = mapper;
		}

		public void Start(string[] args)
		{
			_args = args;
			CheckSetup();
			FireApplicationStartedEvent();
			WriteScore();
			WriteMenu();
		}

		private void FireApplicationStartedEvent()
		{
			_bus.Advanced.SyncBus.Send(_mapper.Map<ApplicationStarted>(_gameConfig));
		}

		private void WriteMenu()
		{
			var menu = new ConsoleMenu(_args, level: 0)
				.Add("Play Easy Game", () => _playGame.Play(GameLevel.Easy))
				.Add("Play Normal Game", () => _playGame.Play(GameLevel.Normal))
				.Add("Play Hard Game", () => _playGame.Play(GameLevel.Hard))
				.Add("Game History", WriteHistory)
				.Add("Exit", (thisMenu) =>
				{
					ExitGame().GetAwaiter().GetResult();
					thisMenu.CloseMenu();
				})
				.Configure(config =>
				{
					config.Selector = "--> ";
					config.EnableFilter = true;
					config.EnableWriteTitle = false;
					config.EnableBreadcrumb = false;
					config.WriteHeaderAction = WriteScore;
					config.ItemForegroundColor = ConsoleColor.DarkGray;
					config.SelectedItemBackgroundColor = ConsoleColor.Black;
					config.SelectedItemForegroundColor = ConsoleColor.Red;
				});

			menu.Show();
		}

		private async Task ExitGame()
		{
			Console.Clear();

			Console.WriteLine("Please wait while exiting...");

			await _bus.Send(_mapper.Map<ApplicationFinishing>(_gameConfig));

			await Task.Delay(TimeSpan.FromSeconds(5));
		}


		private void WriteScore()
		{
			ConsoleWriteWithColor(ConsoleColor.Blue, @"----------------
The Console Game
----------------");
			Console.WriteLine();

			ConsoleWriteWithColor(ConsoleColor.Gray, "Match Counts (Won/Total) : ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfWins);
			ConsoleWriteWithColor(ConsoleColor.Gray, " / ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfPlays);

			ConsoleWriteWithColor(ConsoleColor.Gray, "   Score : ");
			ConsoleWriteWithColor(ConsoleColor.Green, _gameConfig.UserPoints);

			Console.WriteLine();

			ConsoleWriteWithColor(ConsoleColor.Gray, "                    Easy : ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfEasyWins);
			ConsoleWriteWithColor(ConsoleColor.Gray, " / ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfEasyPlays);

			ConsoleWriteWithColor(ConsoleColor.Gray, "   Normal : ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfNormalWins);
			ConsoleWriteWithColor(ConsoleColor.Gray, " / ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfNormalPlays);

			ConsoleWriteWithColor(ConsoleColor.Gray, "   Hard : ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfHardWins);
			ConsoleWriteWithColor(ConsoleColor.Gray, " / ");
			ConsoleWriteWithColor(ConsoleColor.White, _gameConfig.NumberOfHardPlays);

			Console.WriteLine();
		}

		private void ConsoleWriteWithColor(ConsoleColor color, object message)
		{
			var old = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.Write(message);
			Console.ForegroundColor = old;
		}

		private void WriteHistory()
		{
			Console.Clear();
			Console.WriteLine("Game History");
			Console.WriteLine(new string('-', 30));

			var history = _dbManager.ListOfLastNItem(_gameConfig.UserId, 10);
			var table = new ConsoleTable(
				"Level",
				"Date / Time",
				"Duration",
				"Status",
				"Game Point",
				"Iteration Number",
				"Number Held");
			table.Options.EnableCount = false;

			foreach (var item in history)
			{
				table.AddRow(
					item.Level.ToString(),
					item.FinishTime,
					item.FinishTime - item.StartTime,
					(item.IsTheWinner ?? false) ? "Won" : "Lost",
					item.GamePoint,
					item.IterationNumber,
					item.PcNumber);
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
			_bus.Send(_mapper.Map<SaveGameConfig>(_gameConfig));
			Console.Clear();
		}
	}
}