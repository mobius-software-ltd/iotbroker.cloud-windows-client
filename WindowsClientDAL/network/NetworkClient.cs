using com.mobius.software.windows.iotbroker.mqtt;
using com.mobius.software.windows.iotbroker.mqtt.avps;
using com.mobius.software.windows.iotbroker.mqtt.headers.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.network
{
    public interface NetworkClient
    {
        void SetListener(ClientListener listener);

        Boolean createChannel();

        void CloseChannel();

        void Connect();

        Boolean Disconnect();

        void Subscribe(Topic[] topics);

        void Unsubscribe(String[] topics);

        void Publish(Topic topic, byte[] content, Boolean retain, Boolean dup);

        void CloseConnection();

        void CancelConnection();

        void SetState(ConnectionState state);
    }
}
