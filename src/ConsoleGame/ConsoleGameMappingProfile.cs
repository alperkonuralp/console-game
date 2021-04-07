using AutoMapper;
using ConsoleGame.Entities;
using ConsoleGame.Events;
using System;

namespace ConsoleGame
{
	public class ConsoleGameMappingProfile : Profile
	{
		private static readonly DateTime _zeroDatetime = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.FromHours(0)).DateTime;

		public ConsoleGameMappingProfile()
		{
			CreateMap<PlayHistory, GameStarting>();
			CreateMap<PlayHistory, GameFinished>()
				.ForMember(d => d.Game, m => m.Ignore());
			CreateMap<GameConfig, GameFinishedGame>();

			CreateMap<GameStarting, SavePlayHistory>()
				.ForMember(d => d.Status, m => m.MapFrom(_=> GameStatus.Started))
				.ForMember(d => d.FinishTime, m => m.Ignore())
				.ForMember(d => d.IsTheWinner, m => m.Ignore())
				.ForMember(d => d.GamePoint, m => m.Ignore())
				.ForMember(d => d.IterationNumber, m => m.MapFrom(_ => -1))
				.ForMember(d => d.Shoots, m => m.MapFrom(_ => Array.Empty<int>()))
				;
			CreateMap<GameFinished, SavePlayHistory>()
				.ForMember(d => d.Status, m => m.MapFrom(_ => GameStatus.Finished))
				;

			CreateMap<GameFinishedGame, GameConfig>();
			CreateMap<GameFinishedGame, SaveGameConfig>();
			CreateMap<GameConfig, SaveGameConfig>();
			CreateMap<PlayHistory, SavePlayHistory>();

			CreateMap<GameConfig, ApplicationStarted>();
			CreateMap<ApplicationStarted, SaveGameConfig>();
			CreateMap<GameConfig, ApplicationFinishing>();
			CreateMap<ApplicationFinishing, SaveGameConfig>();
		}
	}
}