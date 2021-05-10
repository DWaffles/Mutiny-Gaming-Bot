using System;

namespace MutinyBot.Services
{
    public interface IEventService
    {
        public void Initialize();
    }
    public class EventService : IEventService
    {
        public MutinyBot MutinyBot { protected get; set; }
        public EventService(MutinyBot mutinyBot)
        {
            MutinyBot = mutinyBot;
            Console.WriteLine("EVENTSERVICE");
        }
        public void Initialize()
        {
            if (MutinyBot.Config.Debug)
                Console.WriteLine("EVENTSERVICE INIT");
            
        }
    }
}