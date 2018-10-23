

using com.mobius.software.windows.iotbroker.amqp.codes;
using com.mobius.software.windows.iotbroker.amqp.constructors;
using com.mobius.software.windows.iotbroker.amqp.headerapi;
using com.mobius.software.windows.iotbroker.amqp.tlv.api;
using com.mobius.software.windows.iotbroker.amqp.tlv.compound;
using com.mobius.software.windows.iotbroker.amqp.tlv.described;
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

namespace com.mobius.software.windows.iotbroker.amqp.terminus
{
    public class AMQPSource
    {
        #region private fields

        private String _address;
        private TerminusDurability? _durable;
        private TerminusExpiryPolicy? _expiryPeriod;
        private Int64? _timeout;
        private Boolean? _dynamic;
        private Dictionary<AMQPSymbol, Object> _dynamicNodeProperties;
        private DistributionMode? _distributionMode;
        private Dictionary<AMQPSymbol, Object> _filter;
        private AMQPOutcome _defaultOutcome;
        private List<AMQPSymbol> _outcomes;
        private List<AMQPSymbol> _capabilities;

        #endregion

        #region constructors

        public AMQPSource()
        { 
        }

        #endregion

        #region public fields

        public TLVList getList()
        {

            TLVList list = new TLVList();

            if (_address != null)
                list.addElement(0, AMQPWrapper<AMQPSymbol>.wrap(_address));
            if (_durable != null)
                list.addElement(1, AMQPWrapper<AMQPSymbol>.wrap((Int64)_durable.Value));
            if (_expiryPeriod != null)
                list.addElement(2, AMQPWrapper<AMQPSymbol>.wrap(new AMQPSymbol(StringEnum.GetStringValue(_expiryPeriod.Value))));
            if (_timeout != null)
                list.addElement(3, AMQPWrapper<AMQPSymbol>.wrap(_timeout));
            if (_dynamic != null)
                list.addElement(4, AMQPWrapper<AMQPSymbol>.wrap(_dynamic));

            if (_dynamicNodeProperties != null)
                if (_dynamic != null)
                {
                    if (_dynamic.Value)
                        list.addElement(5, AMQPWrapper<AMQPSymbol>.wrapMap(_dynamicNodeProperties));
                    else
                        throw new MalformedMessageException("Source's dynamic-node-properties can't be specified when dynamic flag is false");
                }
                else
                    throw new MalformedMessageException("Source's dynamic-node-properties can't be specified when dynamic flag is not set");

            if (_distributionMode != null)
                list.addElement(6, AMQPWrapper<AMQPSymbol>.wrap(new AMQPSymbol(StringEnum.GetStringValue(_distributionMode.Value))));

            if (_filter != null)
                list.addElement(7, AMQPWrapper<AMQPSymbol>.wrapMap(_filter));

            if (_defaultOutcome != null)
                list.addElement(8, _defaultOutcome.getList());

            if (_outcomes != null)
                list.addElement(9, AMQPWrapper<AMQPSymbol>.wrapArray(_outcomes));

            if (_capabilities != null)
                list.addElement(10, AMQPWrapper<AMQPSymbol>.wrapArray(_capabilities));

            DescribedConstructor constructor = new DescribedConstructor(list.Code,new TLVFixed(AMQPType.SMALL_ULONG, new byte[] { 0x28 }));
            list.Constructor = constructor;

            return list;
        }

        public void fill(TLVList list)
        {

            if (list.getList().Count > 0)
            {
                TLVAmqp element = list.getList()[0];
                if (!element.isNull())
                    _address = AMQPUnwrapper<AMQPSymbol>.unwrapString(element);
            }
            if (list.getList().Count > 1)
            {
                TLVAmqp element = list.getList()[1];
                if (!element.isNull())
                    _durable = (TerminusDurability)AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }
            if (list.getList().Count > 2)
            {
                TLVAmqp element = list.getList()[2];
                if (!element.isNull())
                    _expiryPeriod = (TerminusExpiryPolicy)StringEnum.Parse(typeof(TerminusExpiryPolicy),AMQPUnwrapper<AMQPSymbol>.unwrapSymbol(element).Value);
            }
            if (list.getList().Count > 3)
            {
                TLVAmqp element = list.getList()[3];
                if (!element.isNull())
                    _timeout = AMQPUnwrapper<AMQPSymbol>.unwrapUInt(element);
            }
            if (list.getList().Count > 4)
            {
                TLVAmqp element = list.getList()[4];
                if (!element.isNull())
                    _dynamic = AMQPUnwrapper<AMQPSymbol>.unwrapBool(element);
            }
            if (list.getList().Count > 5)
            {
                TLVAmqp element = list.getList()[5];
                if (!element.isNull())
                {
                    if (_dynamic != null)
                    {
                        if (_dynamic.Value)
                            _dynamicNodeProperties = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
                        else
                            throw new MalformedMessageException("Received malformed Source: dynamic-node-properties can't be specified when dynamic flag is false");
                    }
                    else
                        throw new MalformedMessageException("Received malformed Source: dynamic-node-properties can't be specified when dynamic flag is not set");
                }
            }
            if (list.getList().Count > 6)
            {
                TLVAmqp element = list.getList()[6];
                if (!element.isNull())
                    _distributionMode = (DistributionMode)StringEnum.Parse(typeof(DistributionMode),AMQPUnwrapper<AMQPSymbol>.unwrapSymbol(element).Value);
            }
            if (list.getList().Count > 7)
            {
                TLVAmqp element = list.getList()[7];
                if (!element.isNull())
                    _filter = AMQPUnwrapper<AMQPSymbol>.unwrapMap(element);
            }
            if (list.getList().Count > 8)
            {
                TLVAmqp element = list.getList()[8];
                if (!element.isNull())
                {
                    AMQPType code = element.Code;
                    if (code != AMQPType.LIST_0 && code != AMQPType.LIST_8 && code != AMQPType.LIST_32)
                        throw new MalformedMessageException("Expected type 'OUTCOME' - received: " + element.Code);

                    _defaultOutcome = AMQPFactory.getOutcome((TLVList)element);
                    _defaultOutcome.fill((TLVList)element);
                }
            }
            if (list.getList().Count > 9)
            {
                TLVAmqp element = list.getList()[9];
                if (!element.isNull())
                    _outcomes = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }
            if (list.getList().Count > 10)
            {
                TLVAmqp element = list.getList()[10];
                if (!element.isNull())
                    _capabilities = AMQPUnwrapper<AMQPSymbol>.unwrapArray(element);
            }
        }

        public String Address
        {
            get
            {
                return _address;
            }

            set
            {
                this._address = value;
            }
        }

        public TerminusDurability? Durable
        {
            get
            {
                return _durable;
            }

            set
            {
                this._durable = value;
            }
        }

        public TerminusExpiryPolicy? ExpiryPeriod
        {
            get
            {
                return _expiryPeriod;
            }

            set
            {
                this._expiryPeriod = value;
            }
        }

        public Int64? Timeout
        {
            get
            {
                return _timeout;
            }

            set
            {
                this._timeout = value;
            }
        }

        public Boolean? Dynamic
        {
            get
            {
                return _dynamic;
            }

            set
            {
                this._dynamic = value;
            }
        }

        public Dictionary<AMQPSymbol,Object> DynamicNodeProperties
        {
            get
            {
                return _dynamicNodeProperties;
            }
        }

        public void addDynamicNodeProperty(String key, Object value)
        {
            if (key == null)
                throw new ArgumentException("Properties map cannot contain objects with null keys");

            if (_dynamicNodeProperties == null)
                _dynamicNodeProperties = new Dictionary<AMQPSymbol, Object>();

            _dynamicNodeProperties[new AMQPSymbol(key)] = value;
        }

        public DistributionMode? DistributionMode
        {
            get
            {
                return _distributionMode;
            }

            set
            {
                this._distributionMode = value;
            }
        }

        public Dictionary<AMQPSymbol, Object> Filters
        {
            get
            {
                return _filter;
            }
        }

        public void addFilter(String key, Object value)
        {
            if (key == null)
                throw new ArgumentException("Filters map cannot contain objects with null keys");

            if (_filter == null)
                _filter = new Dictionary<AMQPSymbol, Object>();

            _filter[new AMQPSymbol(key)] = value;
        }

        public AMQPOutcome DefaultOutcome
        {
            get
            {
                return _defaultOutcome;
            }

            set
            {
                this._defaultOutcome = value;
            }
        }

        public List<AMQPSymbol> Outcomes
        {
            get
            {
                return _outcomes;
            }
        }

        public void addOutcomes(String[] outcomes)
        {
            if (_outcomes == null)
                _outcomes = new List<AMQPSymbol>();

            foreach (String outcome in outcomes)
                _outcomes.Add(new AMQPSymbol(outcome));
        }

        public List<AMQPSymbol> Capabilities
        {
            get
            {
                return _capabilities;
            }
        }

        public void addCapabilities(String[] capabilities)
        {
            if (_capabilities == null)
                _capabilities = new List<AMQPSymbol>();

            foreach (String capability in capabilities)
                _capabilities.Add(new AMQPSymbol(capability));
        }

        #endregion
    }
}
