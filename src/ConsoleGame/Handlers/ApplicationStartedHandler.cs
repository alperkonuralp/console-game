using AutoMapper;
using ConsoleGame.Events;
using Rebus.Bus;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.Handlers
{
	public class ApplicationStartedHandler : IHandleMessages<ApplicationStarted>
	{
		private readonly IBus _bus;
		private readonly IMapper _mapper;

		public ApplicationStartedHandler(IBus bus, IMapper mapper)
		{
			_bus = bus;
			_mapper = mapper;
		}

		public async Task Handle(ApplicationStarted message)
		{
			await _bus.Send(_mapper.Map<SaveGameConfig>(message));
		}
	}
}