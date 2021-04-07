using AutoMapper;
using ConsoleGame.Events;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.Handlers
{
	public class ApplicationStartedHandler : IHandleMessages<ApplicationStarted>
	{
		private readonly IBus _bus;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		public ApplicationStartedHandler(
			IBus bus
			, IMapper mapper
			, ILogger<ApplicationFinishingHandler> logger)
		{
			_bus = bus;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task Handle(ApplicationStarted message)
		{
			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Starting");

			await _bus.Send(_mapper.Map<SaveGameConfig>(message));

			_logger.LogInformation("{@ClassName}.{@MethodName} {@Status}.", GetType().FullName, nameof(Handle), "Finished");
		}
	}
}