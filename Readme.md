# wipm.exchangestats

Play sytem which is broken up into multiple services that communicate via message passing.  Azure Service bus is used to pass messages between the services.


## Domain

The domain is financial information from stock markets.  It will accept updates and then update various statistics from the information.


## Areas

* **Data ingress** - Responsible for loading raw data into the system.
* **Audit** - Respoonsible providing an audit trail for requests and primary objects.



## Architectural

The architecture is intended to be:

* Service Oriented
* Asynchronous communication
* CQRS base structure.