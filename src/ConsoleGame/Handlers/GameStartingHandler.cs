using AutoMapper;
using ConsoleGame.Events;
using Rebus.Bus;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.Handlers
{
	public class GameStartingHandler : IHandleMessages<GameStarting>
	{
		private readonly IBus _bus;
		private readonly IMapper _mapper;

		public GameStartingHandler(IBus bus, IMapper mapper)
		{
			_bus = bus;
			_mapper = mapper;
		}

		public async Task Handle(GameStarting message)
		{
			await _bus.Send(_mapper.Map<SavePlayHistory>(message));
		}
	}
}