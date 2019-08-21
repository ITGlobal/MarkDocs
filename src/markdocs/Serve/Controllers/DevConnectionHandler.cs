using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace ITGlobal.MarkDocs.Tools.Serve.Controllers
{
    public class DevConnectionHandler : ConnectionHandler
    {
        private readonly DevConnectionManager _manager;

        public DevConnectionHandler(DevConnectionManager manager)
        {
            _manager = manager;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            _manager.OnConnectedAsync(connection);
            await ProcessRequests(connection);
            _manager.OnDisconnectedAsync(connection);
        }

        public async Task ProcessRequests(ConnectionContext connection)
        {
            while (true)
            {
                var result = await connection.Transport.Input.ReadAsync();
                var buffer = result.Buffer;
                try
                {
                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    connection.Transport.Input.AdvanceTo(buffer.End);
                }
            }
        }
    }
}
