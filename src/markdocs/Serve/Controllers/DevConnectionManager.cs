using System.Buffers;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Tools.Serve.Controllers
{
    public sealed class DevConnectionManager
    {
        private readonly object _connectionsLock = new object();
        private readonly List<ConnectionContext> _connections = new List<ConnectionContext>();
        
        public void OnConnectedAsync(ConnectionContext connection)
        {
            lock (_connectionsLock)
            {
                _connections.Add(connection);
            }
        }

        public void OnDisconnectedAsync(ConnectionContext connection)
        {
            lock (_connectionsLock)
            {
                _connections.Remove(connection);
            }
        }

        public void Publish<T>(T data)
        {
            lock (_connectionsLock)
            {
                if (_connections.Count == 0)
                {
                    return;
                }

                var buffer = Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(data)
                );

                foreach (var connection in _connections)
                {
                    connection.Transport.Output.Write(buffer);
                    connection.Transport.Output.FlushAsync().GetAwaiter().GetResult();
                }
            }
        }
    }
}