# Bulkie - Reference Application

Dapr event-driven reference application.

![High level](docs/images/bulkie.png)


## What does it do?
* Imports a batch of files ("Bulkie") into the system.
* Tracks status of imports

## How is it implemented?

Dapr Building blocks: 
* Virtual actors
* Pub/Sub
* Actor state

Event driven:
* Status updates
* File processing submissions


Below shows a sequence diagram of the flow of the creation of a Bulkie:

![Sequence diagram](docs/images/bulkie-seq.png)
