EventStore Projection Examples
==============================

Some fun stuff with EventStore JavaScript projections

Run EventStore
==============

Ensure to run EventStore as administrator and to use parameter enable projections when running he event store

Run following statement from command line to run EventStore with projections and an in-memory database

```
lib\EventStore\EventStore.SingleNode.exe --run-projections=all --mem-db
```

NOTE: These examples are based on the latest version at the time of writing. (16-jan-2014) For instructions on how to build the EventStrore see <a href="https://github.com/EventStore/EventStore">the official github page</a>.

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
