# DataIngress

Accepts new data and validates it before making it available to the system as a whole.

## Components

* Gateway  
* Listener 

### Gateway

This is a REST based web api that accepts raw data. It provides the access point for external system to supply raw data for entry into the system. The main responsibilities are:

* Authorise requests
* Make request available to the listener 

#### api

* /v1/exchanges


##### POST /v1/exchanges

Allows one or more exchange to the added to the system. The request should be:

```
POST /v1/exchanges HTTP/1.1
Host: localhost:64786
Content-Type: application/json
Cache-Control: no-cache

[
	{
  "Code": "NYSE",
  "Name": "New York Stock Exchange"
   }
]
```

In the **????** header of the response is the url you can use to look up the status of the request.


### Listener

Windows service that listens for new raw data messages from the gateway, processes the request and reports the state changes and any errors. The main responsibilities are:

* Process raw data request into state change event and errors.


## System Infrastructure

* An azure service bus namespace with the following:
  * **ingressGateway** topic - this is the interface between the _Gateway_ and the _Listener_.
	 *	**dataIngressListener** subscription - used by the _Listener_ to receive the new data.
  * **ingressData** topic - this is where the listener puts the state change events caused by the _ingressGateway_ messages.

**NOTE** All _topics_ should have duplicate message detection turn on the "duplicate detection history" need to be set to a period longer than the expected maximum system down time. 

* An sql server database


## Projects

* wipm.exchangestates.data.ingress.gateway 
* wipm.exchangestates.data.ingress.listenern 
* wipm.exchangestates.data.ingress.core
* wipm.exchangestates.data.ingress.interfaces 


### wipm.exchangestates.data.ingress.gateway 

Web api application that implements the **Gateway** component. 

### Configuration settings

#### _Web.Secret.Config_

* _ingress_gateway_service_bus_connection_string_, This is the connection string for the azure service bus namspace that the _ingressGateway_ topic is on.

**Note** the Web.Secret.Config file is not checked into source controll because the settings could hold sensative information so this will need to create after cloning the repository.

```
<appSettings>
  <!-- Ingress Queue-->
  <add key="ingress_gateway_service_bus_connection_string" value="???" />
</appSettings>
```

### wipm.exchangestates.data.ingress.listener

Windows service implmeneted using a console application and _Topshelf_ that implements the **Listener** component. 
 

### Configuration settings

#### _App.Secret.Config_

* _ingress_gateway_service_bus_connection_string_, This is the connection string for the azure service bus namspace that the _ingressGateway_ topic is on.
* _ingress_data_service_bus_connection_string_, This is the connection string for the azure service bus namspace that the _ingressData_ topic is on.

**Note** the App.Secret.Config file is not checked into source control because the settings could hold sensative information so this will need to create after cloning the repository.

```
<appSettings>
  <!-- Ingress Queue-->
  <add key="ingress_gateway_service_bus_connection_string" value="???" />
  <add key="ingress_data_service_bus_connection_string" value="???" />
</appSettings>
```

#### _ConnectionString.Secret.Config_

* _DataModelDbContext_, This is the connection string to the sql server database for the _listener_.

**Note** the Connection.Secret.Config file is not checked into source control because the settings could hold sensative information so this will need to create after cloning the repository.

```
<connectionStrings>
  <add name="DataModelDbContext" connectionString="" />
</connectionStrings>
```

### wipm.exchangestates.data.ingress.core

Library that contains the core bussiness logic for the listener, the idea is that this is easy to unit test and the listern just implements the service infrastructure which is not so easy to unit test.

### wipm.exchangestates.data.ingress.interfaces 

Library that contains the common code ( classes, interfaces, classes ) that are shared between the Gateway and Listener.  It is ok for these two components to share code as they are intended to be deployed together, **This library should not be referenced by any other projects**.

