using AutoMapper;
using ConsoleGame.Events;
using Rebus.Bus;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.Handlers
{
	public class GameFinishedHandler : IHandleMessages<GameFinished>
	{
		private readonly IBus _bus;
		private readonly IMapper _mapper;

		public GameFinishedHandler(IBus bus, IMapper mapper)
		{
			_bus = bus;
			_mapper = mapper;
		}

		public async Task Handle(GameFinished message)
		{
			await Task.WhenAll(
				_bus.Send(_mapper.Map<SaveGameConfig>(message.Game)),
				_bus.Send(_mapper.Map<SavePlayHistory>(message))
			);
		}
	}
}