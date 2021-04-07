using AutoMapper;
using ConsoleGame.Events;
using Rebus.Bus;
using Rebus.Handlers;
using System.Threading.Tasks;

namespace ConsoleGame.Handlers
{
	public class ApplicationFinishingHandler : IHandleMessages<ApplicationFinishing>
	{
		private readonly IBus _bus;
		private readonly IMapper _mapper;

		public ApplicationFinishingHandler(IBus bus, IMapper mapper)
		{
			_bus = bus;
			_mapper = mapper;
		}

		public async Task Handle(ApplicationFinishing message)
		{
			await _bus.Send(_mapper.Map<SaveGameConfig>(message));
		}
	}
}