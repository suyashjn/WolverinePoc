using Wolverine;
using Wolverine.Http;
using WolverinePOC.Messages;

namespace WolverinePOC.Controllers
{
    public static class MesssagesController
    {
        // POST /publish
        [WolverinePost("/publish")]
        public static async Task<IResult> PublishMessage(
            MyMessage message,
            IMessageBus bus)
        {
            // Publish the message to the default queue ("my_queue")
            await bus.EndpointFor("my_queue").SendAsync(message);

            return Results.Accepted();
        }
    }
}
