EventStore Projection Examples
==============================

Some fun stuff with EventStore JavaScript projections.

Projections and Tests
=====================

If you are only interssted in the Javascript Projection and their tests you can have a look here:

https://github.com/tim-cools/EventStore-Examples/tree/master/src/Soloco.EventStore.MeasurementProjections/Projections/Sources

Run EventStore
==============

To run the example, ensure to run the EventStore as administraor and with the <em>--run-projections=all</em> flag set. The following statement from command line runs the EventStore with projections enabled and an in-memory database.


```
lib\EventStore\EventStore.SingleNode.exe --run-projections=all --mem-db
```

NOTE: The examples are based on the latest version from the main branch at the time of writing (v3.0.0rc1). Because I noticed that it is rather important to use the Client API against the same version of the EventStore, I included the binaries in the github repository. This allows you to check out the examples and run them, without worrying about the compatible versions or building the EventStore yourself.

For instructions on how to build the EventStrore see <a href="https://github.com/EventStore/EventStore">the official github page</a>.

Run an Example
==============

The projections in these examples are managed by using the ClientAPI from C#. Each example will automatically:
* Create the necessary projection(s)
* Simulate the necessary events
* And show you the output of the projections

Blog Posts Series
=================

These examples are explained in a series of Blog post: http://www.soloco.be/eventstore-projections-by-example-series/

Have Fun!
=========
