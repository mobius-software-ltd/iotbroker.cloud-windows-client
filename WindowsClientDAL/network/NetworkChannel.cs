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
    public interface NetworkChannel<T>
    {
        void Send(T message);

        Boolean Init(ConnectionListener<T> listener);

        Boolean IsConnected();

        void Close();

        void Shutdown();
    }
}
