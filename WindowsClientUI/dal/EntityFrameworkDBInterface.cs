/**
 * Mobius Software LTD
 * Copyright 2015-2017, Mobius Software LTD
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
 
using com.mobius.software.windows.iotbroker.dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mobius.software.windows.iotbroker.mqtt.avps;
using System.Data.Entity;

namespace com.mobius.software.windows.iotbroker.ui.win7.dal
{
    public class EntityFrameworkDBInterface : DBInterface
    {
        Account defaultAccount = null;

        public EntityFrameworkDBInterface(Account account)
        {
            this.defaultAccount = account;
        }

        public void DeleteAllTopics()
        {
            MQTTModel mqtt = new MQTTModel();
            var topics = from t in mqtt.Topics where t.Account.ID == defaultAccount.ID select t;
            if (topics.Count() > 0)
                mqtt.Topics.RemoveRange(topics);

            mqtt.SaveChanges();
        }

        public void DeleteTopic(string topicName)
        {
            MQTTModel mqtt = new MQTTModel();
            var topics = from t in mqtt.Topics where t.TopicName == topicName where t.Account.ID == defaultAccount.ID select t;
            if (topics.Count() > 0)
                mqtt.Topics.Remove(topics.First());

            mqtt.SaveChanges();
        }

        public void StoreTopic(string topicName, QOS qos)
        {
            MQTTModel model = new MQTTModel();
            var topics = from t in model.Topics where t.TopicName == topicName where t.Account.ID == defaultAccount.ID select t;
            if (topics.Count() > 0)
            {
                Topic currTopic = topics.First();
                currTopic.QOS = qos;
                model.Topics.Attach(currTopic);
                var entry = model.Entry(currTopic);
                entry.Property(t => t.QOS).IsModified = true;
                model.SaveChanges();
            }
            else
            { 
                Topic newTopic = new Topic();
                newTopic.QOS = qos;
                newTopic.TopicName = topicName;
                newTopic.Account = defaultAccount;

                model.Accounts.Attach(defaultAccount);         
                model.Topics.Add(newTopic);
                model.Entry(defaultAccount).State = EntityState.Unchanged;
                model.SaveChanges();
            }
        }

        public bool TopicExists(string topicName)
        {
            MQTTModel mqtt = new MQTTModel();
            var topics = from t in mqtt.Topics where t.TopicName == topicName where t.Account.ID == defaultAccount.ID select t;

            return topics.Count() > 0;
        }

        public void StoreMessage(string topicName, byte[] content, QOS qos)
        {
           Message newMessage = new Message();
            newMessage.QOS = qos;
            newMessage.Content = content;
            newMessage.TopicName = topicName;
            newMessage.Incoming = true;
            newMessage.Account = defaultAccount;

            MQTTModel model = new MQTTModel();
            model.Accounts.Attach(defaultAccount);
            model.Messages.Add(newMessage);
            model.Entry(defaultAccount).State = EntityState.Unchanged;
            model.SaveChanges();
        }

        public void UnmarkAsDefault(Account account)
        {
            MQTTModel mqtt = new MQTTModel();
            account.IsDefault = false;
            mqtt.Accounts.Attach(account);
            var entry = mqtt.Entry(account);
            entry.Property(a => a.IsDefault).IsModified = true;
            mqtt.SaveChanges();
        }
    }
}
