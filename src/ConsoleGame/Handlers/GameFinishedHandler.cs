using AutoMapper;
using ConsoleGame.Events;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.Handlers
{
	public class GameFinishedHandler : IHandleMessages<GameFinished>
	{
		private readonly IBus _bus;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		public GameFinishedHandler(
			IBus bus
			, IMapper mapper
			, ILogger<ApplicationFinishingHandler> logger)
		{
			_bus = bus;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task Handle(GameFinished message)
		{
			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Starting");
			
			await Task.WhenAll(
				_bus.Send(_mapper.Map<SaveGameConfig>(message.Game)),
				_bus.Send(_mapper.Map<SavePlayHistory>(message))
			);
			
			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Finished");
		}
	}
}