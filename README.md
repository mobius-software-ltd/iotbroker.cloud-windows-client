# IoTBroker.Cloud Windows Client

IoTBroker.Cloud is a Windows client which allows to connect to MQTT server. IoTBroker.Cloud Windows client sticks to [MQTT 3.1.1](http://docs.oasis-open.org/mqtt/mqtt/v3.1.1/os/mqtt-v3.1.1-os.pdf) standards. 

## Features

* **Clean / persistent session.** When the client disconnects, its session state can be stored (if you set Clean session flag to false) or removed (if you set Clean session flag to true). The session state includes subscriptions and incoming QoS 1 and QoS 2 messages while the client is off.

* **Last Will and Testament.** This feature implies that if a client goes offline without sending DISCONNECT message (due to some failure), other clients will be notified about that.

* **Keep Alive.** If Keep Alive is higher than 0, the client and the server is constantly exchanging PING messages to make sure whether the opposite side is still available. 

* **Retain messages.** It allows to "attach" a message to a particular topic, so the new subscribers become immediately aware of the last known state of a topic.

* **Assured message delivery.** Each message is sent according to the level of Quality of Service (QoS). 3 QoS levels are supported:
- QoS 0 (At most once) — a message is sent only one time. 
- QoS 1 (At least once) — a message is sent at least one time.
- QoS 2 (Exactly once) — a message is sent exactly one time.

## Getting Started

These instructions will help you get a copy of the project and run it.

### Prerequisites

[Visual Studio](https://www.visualstudio.com/downloads) should be installed before starting to clone IoTBroker.Cloud Windows Client. 

### Installation
* To install IoTBroker.Cloud, first you should clone [IotBroker.Cloud Windows Client](https://github.com/mobius-software-ltd/iotbroker.cloud-windows-client.git).

* In order to open IoTBroker.Cloud Windows client in Visual Studio, you should go to *File — Open — Project / Solution*. Then choose the **WindowsClient.sln** file.

* Finally you should press **Start** sign to run the project. If the procedure is successful, you will see the Login page in the form of pop-up window. Now you are able to log in and to start exchanging messages with MQTT server.

Please note that at this stage it is not possible to register as a client. You can only log in to your existing account.

IoTBroker.Cloud Windows Client is developed by [Mobius Software](http://mobius-software.com).

## [License](LICENSE.md)
