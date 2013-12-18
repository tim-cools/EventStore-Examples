/// <reference path="References\1Prelude.js" />

var measurementReadRollingAveragPerWeekDay = function measurementReadRollingAveragPerWeekDayConstructor($eventServices) {
    
    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    
    var eventStreamId = function (originalStreamId) {
        return 'MeasurementPeriod-' + originalStreamId + '+H';
    };

    var getWeekDay = function (timestamp) {
        var date = new Date(timestamp);
        return days[date.getDay()];
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
    var withFourDigits = function(value) {
        return Math.round(value * 10000) / 10000;
    };
    
    var handler = function (previousState, measurementEvent) {

        if (measurementEvent.body == null) return previousState;

        var timeSlot = measurementEvent.body.Timeslot;
        var newAverage = measurementEvent.body.Average;
        var weekday = getWeekDay(timeSlot);
        
        if (!previousState[weekday]) {
            previousState[weekday] = { values: [], average: 0, total: 0 };
        }

        var weekdayState = previousState[weekday];
        weekdayState.values.push(newAverage);
        
        var numberOfValues = weekdayState.values.length;
        weekdayState.total = withFourDigits(weekdayState.total + newAverage);
        
        if (numberOfValues == 8) {
            weekdayState.total -= weekdayState.values.shift();
        }

        weekdayState.average = withFourDigits(weekdayState.total / numberOfValues);

        return previousState;
    };
    
    return {
        getWeekDay: getWeekDay,
        handleEvent: handler
    };
};

fromCategory('MeasurementAverageDayMeter')
  .foreachStream()
  .when( {
      MeasurementAverageDay: measurementReadRollingAveragPerWeekDay().handleEvent
  });
