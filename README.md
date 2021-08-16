# Bulkie - Reference Application

Dapr event-driven reference application.

![High level](docs/images/bulkie.png)


## What does it do?
* Imports files into the system
* Tracks status of imports

## How is it implemented?

Dapr Building blocks: 
* Virtual actors
* Pub/Sub
* Actor state

Event driven:
* Status updates
* File processing submissions


Below shows a sequence diagram of the flow of the creation Bulkie process:

![Sequence diagram](docs/images/bulkie-seq.png)
