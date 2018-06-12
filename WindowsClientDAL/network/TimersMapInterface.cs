using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.network
{
    public interface TimersMapInterface<T>
    {
        void CancelConnectTimer();

        void RefreshTimer(MessageResendTimer<T> timer);
    }
}
