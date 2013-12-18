/// <reference path="References\1Prelude.js" />

var measurementReadRollingAveragPerWeekDay = function measurementReadRollingAveragPerWeekDayConstructor($eventServices) {
    
    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var eventStreamId = function (originalStreamId) {
        return 'MeasurementPeriod-' + originalStreamId + '+H';
    };

    var createEvent = function(timeslot, reading, count) {
        return {
            Timeslot: timeslot,
            Total: reading,
            Count: count,
            Average: reading / count
        };
    };

    var createState = function(reading, count, timestamp) {
        return {
            total: reading,                  
            count: count,
            average: reading / count,
            lastTimestamp: timestamp
        };
    };

    var handler = function (previousState, measurementEvent) {

        if (measurementEvent.body == null) return previousState;

        var timeSlot = measurementEvent.body.Timeslot;
        var newAverage = measurementEvent.body.Average;
        var weekday = getWeekDay(timeSlot);
        
        if (!previousState[weekday]) {
            previousState[weekday] = new { values: [], average: 0 };
        }

        var weekdayState = previousState[weekday];
        weekdayState.values.push(newAverage);
        
        var numberOfValues = weekday.values.length;
        var average = weekdayState.average + (newAverage / numberOfValues);
        
        if (numberOfValues == 8) {
            average -= weekday.values.pop() / numberOfValues;
        }

        weekdayState.average = average;
        
        return previousState;
    };
    
    return {
        handleEvent: handler
    };
};

fromCategory('MeasurementAverageDayMeter')
  .foreachStream()
  .when( {
      MeasurementAverageDay: measurementReadRollingAveragPerWeekDay().handleEvent
  });
