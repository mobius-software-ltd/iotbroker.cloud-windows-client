

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.fixed_;
using com.mobius.software.windows.iotbroker.amqp.wrappers;
using com.mobius.software.windows.iotbroker.dal;
using com.mobius.software.windows.iotbroker.mqtt.exceptions;
/**
* Mobius Software LTD
* Copyright 2015-2018, Mobius Software LTD
*
* This is free software; you can redistribute it and/or modify it
* under the terms of the GNU Lesser General Public License as
* published by the Free Software Foundation; either version 2.1 of
* the License, or (at your option) any later version.
*
* This software is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
* Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public
* License along with this software; if not, write to the Free
* Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
* 02110-1301 USA, or see the FSF site: http://www.fsf.org.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace com.mobius.software.windows.iotbroker.amqp.headeramqp
{
    public class AMQPOpen : AMQPHeader
    {
        #region private fields

        private String _containerId;
        private String _hostname;
        private Int64? _maxFrameSize;
        private Int32? _channelMax;
        private Int64? _idleTimeout;
        private List<AMQPSymbol> _outgoingLocales;
        private List<AMQPSymbol> _incomingLocales;
        private List<AMQPSymbol> _offeredCapabilities;
        private List<AMQPSymbol> _desiredCapabilities;
        private Dictionary<AMQPSymbol, Object> _properties;

        #endregion

        #region constructors

        public AMQPOpen():base(HeaderCodes.OPEN)
        { 
        }

        #endregion

        #region public fields

        public override TLVList getArguments()
        {
            TLVList list = new TLVList();

            if (_containerId == null)
                throw new MalformedMessageException("Open header's container id can't be null");

            list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_containerId));

            if (_hostname != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap(_hostname));

            if (_maxFrameSize != null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(_maxFrameSize));

            if (_channelMax != null)
                list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_channelMax));

            if (_idleTimeout != null)
                list.addElement(4, AMQPWrapper<AMQPSymbol>.wrap(_idleTimeout));

            if (_outgoingLocales != null)
                list.addElement(5, AMQPWrapper<AMQPSymbol>.wrapArray(_outgoingLocales));

            if (_incomingLocales != null)
                list.addElement(6, AMQPWrapper<AMQPSymbol>.wrapArray(_incomingLocales));

            if (_offeredCapabilities != null)
                list.addElement(7, AMQPWrapper<AMQPSymbol>.wrapArray(_offeredCapabilities));

            if (_desiredCapabilities != null)
                list.addElement(8, AMQPWrapper<AMQPSymbol>.wrapArray(_desiredCapabilities));

            if (_properties != null)
                list.addElement(9, AMQPWrapper<AMQPSymbol>.wrapMap(_properties));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { (byte)_code }));
            list.Constructor = constructor;

            return list;
        }

        public override void fillArguments(TLVList list)
        {
            int size = list.getList().Count;

            if (size == 0)
                throw new MalformedMessageException("Received malformed Open header: container id can't be null");

            if (size > 10)
                throw new MalformedMessageException("Received malformed Open header. Invalid number of arguments: " + size);

            TLVAmqp element = list.getList()[0];
            if (element.isNull())
                throw new MalformedMessageException("Received malformed Open header: container id can't be null");

            _containerId = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);

            if (size > 1)
            {
                element = list.getList()[1];
                if (!element.isNull())
                    _hostname = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }

            if (size > 2)
            {
                element = list.getList()[2];
                if (!element.isNull())
                    _maxFrameSize = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 3)
            {
                element = list.getList()[3];
                if (!element.isNull())
                    _channelMax = AMQPUnwrapper<AMQPSymbol>.unwrapUShort(element);
            }

            if (size > 4)
            {
                element = list.getList()[4];
                if (!element.isNull())
                    _idleTimeout = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }

            if (size > 5)
            {
                element = list.getList()[5];
                if (!element.isNull())
                    _outgoingLocales = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 6)
            {
                element = list.getList()[6];
                if (!element.isNull())
                    _incomingLocales = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 7)
            {
                element = list.getList()[7];
                if (!element.isNull())
                    _offeredCapabilities = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 8)
            {
                element = list.getList()[8];
                if (!element.isNull())
                    _desiredCapabilities = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }

            if (size > 9)
            {
                element = list.getList()[9];
                if (!element.isNull())
                    _properties = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
            }
        }

        public String toString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("Doff: %d \n", _doff));
            sb.Append(String.Format("Type: %d \n", _headerType));
            sb.Append(String.Format("Channel: %d \n", _channel));
            sb.Append("Arguments: \n");
            sb.Append(String.Format("Container-id: %s \n", _containerId));
            sb.Append(String.Format("Hostname: %s \n", _hostname));
            sb.Append(String.Format("Max-frame-size: %s \n", _maxFrameSize));
            sb.Append(String.Format("Channel max: %d \n", _channelMax));
            sb.Append(String.Format("Idle-timeout: %d \n", _idleTimeout));
            sb.Append(String.Format("Outgoing-locales (array of %d elements)", _outgoingLocales.Count));
            sb.Append(_outgoingLocales);
            sb.Append("\n");
            sb.Append(String.Format("Incoming-locales (array of %d elements)", _incomingLocales.Count));
            sb.Append(_incomingLocales);
            sb.Append("\n");
            sb.Append(String.Format("Offered capabilities (array of %d elements)", _offeredCapabilities.Count));
            sb.Append(_offeredCapabilities);
            sb.Append("\n");
            sb.Append(String.Format("Desired capabilities (array of %d elements)", _desiredCapabilities.Count));
            sb.Append(_desiredCapabilities);
            sb.Append("\n");
            sb.Append(String.Format("Properties (map of %d elements)", _properties.Count));
            sb.Append(_properties);
            return sb.ToString();
        }

        public String ContainerId
        {
            get
            {
                return _containerId;
            }

            set
            {
                if (value == null)
                    throw new ArgumentException("Container id can't be assigned a null value");

                this._containerId = value;
            }
        }

        public String Hostname
        {
            get
            {
                return _hostname;
            }

            set
            {
                this._hostname = value;
            }
        }

        public Int64? MaxFrameSize
        {
            get
            {
                return _maxFrameSize;
            }

            set
            {
                this._maxFrameSize = value;
            }
        }

        public Int32? ChannelMax
        {
            get
            {
                return _channelMax;
            }

            set
            {
                this._channelMax = value;
            }
        }

        public Int64? IdleTimeout
        {
            get
            {
                return _idleTimeout;
            }

            set
            {
                this._idleTimeout = value;
            }
        }

        public List<AMQPSymbol> OutgoingLocales
        {
            get
            {
                return _offeredCapabilities;
            }
        }

        public void addOutgoingLocale(String[] locales)
        {
            if (_outgoingLocales == null)
                _outgoingLocales = new List<AMQPSymbol>();

            foreach (String locale in locales)
                _outgoingLocales.Add(new AMQPSymbol(locale));
        }

        public List<AMQPSymbol> IncomingLocales
        {
            get
            {
                return _incomingLocales;
            }
        }

        public void addIncomingLocale(String[] locales)
        {
            if (_incomingLocales == null)
                _incomingLocales = new List<AMQPSymbol>();

            foreach (String locale in locales)
                _incomingLocales.Add(new AMQPSymbol(locale));
        }

        public List<AMQPSymbol> OfferedCapabilities
        {
            get
            {
                return _offeredCapabilities;
            }
        }

        public void addOfferedCapability(String[] capabilities)
        {
            if (_offeredCapabilities == null)
                _offeredCapabilities = new List<AMQPSymbol>();

            foreach (String capability in capabilities)
                _offeredCapabilities.Add(new AMQPSymbol(capability));
        }

        public List<AMQPSymbol> DesiredCapabilities
        {
            get
            {
                return _desiredCapabilities;
            }
        }

        public void addDesiredCapability(String[] capabilities)
        {
            if (_desiredCapabilities == null)
                _desiredCapabilities = new List<AMQPSymbol>();

            foreach (String capability in capabilities)
                _desiredCapabilities.Add(new AMQPSymbol(capability));
        }

        public Dictionary<AMQPSymbol, Object> Properties
        {
            get
            {
                return _properties;
            }
        }

        public void addProperties(String key, Object value)
        {
            if (key == null)
                throw new ArgumentException("Properties map cannot contain objects with null keys");

            if (_properties == null)
                _properties = new Dictionary<AMQPSymbol, Object>();

            _properties[new AMQPSymbol(key)] = value;
        }

        public override int getLength()
        {
            int length = 8;
            TLVAmqp arguments = this.getArguments();
            length += arguments.getLength();
            return length;
        }

        public override int getType()
        {
            return (int)HeaderCodes.OPEN;
        }

        public override void ProcessBy(AMQPDevice device)
        {
            device.ProcessOpen(IdleTimeout);
        }

        #endregion
    }
}
