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

NOTE: These examples are based on the latest version at the time of writing. (11-jan-2014) For instructions on how to build the EventStrore see <a href="https://github.com/EventStore/EventStore">the official github page</a>.

Run an Example
==============

The projections in these examples are managed by using the ClientAPI from C#. Each example will automatically:
* Create the necessary projection(s)
* Simulate the necessary events
* And show you the output of the projections

Blog Posts Series
=================

These examples are explained in a series of Blog post: http://www.soloco.be/eventstore-projections-by-example-series/

Possible future posts
=====================

* ClientAPI C# Basics
* Count number of events of a specific type

(re) Partitioning (category)

* Calculate average per day for a category
* Calculate rolling average per weekday based average per day projection 
* Calculate rolling average per hour of week for an aggregate stream

* Alarm when consumption is larger as x for y time
MeasurmentConfigureAlarm x, y (stored in state)
MeasurmentRead { dateWhenAbove }


* Unit Test Projections with Jasmine 
  MVC, bug in R# that prevents debugger;
  
* Share JavaScript files between projections 

* EventStore Projection Patterns
  http://highlyscalable.wordpress.com/2012/02/01/mapreduce-patterns/
  http://shop.oreilly.com/product/0636920025122.do
* Visualize heat-map based on rolling average per hour of week projection with AngularJS
* Create snapshot or read-model (?)

Have Fun!
=========
