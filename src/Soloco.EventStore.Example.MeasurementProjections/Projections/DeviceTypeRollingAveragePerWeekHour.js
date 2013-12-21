/// <reference path="References\1Prelude.js" />

var DeviceTypeRollingAveragePerWeekHour = function measurementReadRollingAveragPerWeekDayConstructor($eventServices) {
    
    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    
    var eventStreamId = function (originalStreamId) {
        return 'MeasurementRollingAveragePerWeekday-' + originalStreamId;
    };

    var getWeekDay = function (timestamp) {
        var date = new Date(timestamp);
        return days[date.getDay()];
    };

    var withDefault = function (value) { return !value || !value.average ? 0 : value.average; };
    
    var createEvent = function (previousState) {
        return {
            Monday: withDefault(previousState.Monday),
            Tuesday: withDefault(previousState.Tuesday),
            Wednesday: withDefault(previousState.Wednesday),
            Thursday: withDefault(previousState.Thursday),
            Friday: withDefault(previousState.Friday),
            Saturday: withDefault(previousState.Saturday),
            Sunday: withDefault(previousState.Sunday),
        };
    };
    
    var withFourDigits = function(value) {
        return Math.round(value * 10000) / 10000;
    };

    var emitRollingAverageEvent = function (measurementEvent, previousState) {

        var name = eventStreamId(measurementEvent.streamId, previousState.lastTimestamp);
        var event = createEvent(previousState);

        eventServices.emit(name, "MeasurementRollingAveragePerWeekday", event);
    };
    
    var ensureDefaultState = function(previousState) {

        if (previousState.Monday) return previousState;

        return {
            Monday: { values: [], average: 0, total: 0 },
            Tuesday: { values: [], average: 0, total: 0 },
            Wednesday: { values: [], average: 0, total: 0 },
            Thursday: { values: [], average: 0, total: 0 },
            Friday: { values: [], average: 0, total: 0 },
            Saturday: { values: [], average: 0, total: 0 },
            Sunday: { values: [], average: 0, total: 0 }
        };
    };
    
    var handler = function (previousState, measurementEvent) {

        if (measurementEvent.body == null) return previousState;

        var timeSlot = measurementEvent.body.Timeslot;
        var newAverage = measurementEvent.body.Average;
        var weekday = getWeekDay(timeSlot);

        var state = ensureDefaultState(previousState);

        var weekdayState = state[weekday];
        weekdayState.values.push(newAverage);
        
        var numberOfValues = weekdayState.values.length;
        weekdayState.total = withFourDigits(weekdayState.total + newAverage);
        
        if (numberOfValues == 8) {
            weekdayState.total -= weekdayState.values.shift();
        }

        weekdayState.average = withFourDigits(weekdayState.total / numberOfValues);

        emitRollingAverageEvent(measurementEvent, state);

        return state;
    };
    
    return {
        getWeekDay: getWeekDay,
        handleEvent: handler
    };
};

fromCategory('DeviceType')
    .foreachStream()
    .when( {
        MeasurementAverageDay: measurementReadRollingAveragPerWeekDay().handleEvent
    });