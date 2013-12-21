/// <reference path="References\1Prelude.js" />

var deviceTypeRollingAveragePerWeekHour = function deviceTypeRollingAveragePerWeekHourConstructor($eventServices) {
    
    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    var rollingAverageNumbers = 100;
    var hoursInADay = 24;

    var eventStreamId = 'DeviceTypeRollingAveragePerWeekHour';
    var eventName = 'DeviceTypeRollingAveragePerWeekHour';

    var getWeekDay = function (timestamp) {
        var date = new Date(timestamp);
        return days[date.getDay()];
    };
    
    var defaultHours = function (hourState) {
        
        var hours = {};
        for (var hour = 0; hour < hoursInADay; hour++) {
            hours[hour] = !hourState[hour] ? hourState[hour].average : 0;
        }
        return hours;
    };
    
    var withDefault = function(dayState) {
         return !dayState || !dayState.average ? defaultHours(dayState) : dayState.average;
    };
    
    var createEvent = function (state) {

        var event = {};
        for (var dayIndex = 0; dayIndex < days.length; dayIndex++) {
            var day = days[dayIndex];
            event[day] = withDefault(state[day]);
        }
        return event;
    };
    
    var withFourDigits = function(value) {
        return Math.round(value * 10000) / 10000;
    };

    var emitRollingAverageEvent = function (measurementEvent, previousState) {

        var event = createEvent(previousState);

        eventServices.emit(eventStreamId, eventName, event);
    };
    
    var defaultAverageHourState = function () {
        return { values: [], average: 0, total: 0 };
    };

    var defaultAverageDayState = function() {

        var hours = {};
        for (var hour = 0; hour < hoursInADay; hour++) {
            hours[hour] = defaultAverageHourState();
        }
        return hours;
    };

    var ensureDefaultState = function (previousState) {

        if (previousState.Monday) return previousState;

        var state = {};
        for (var dayIndex = 0; dayIndex < days.length; dayIndex++) {
            var day = days[dayIndex];
            state[day] = defaultAverageDayState();
        }
        return state;
    };

    var addNewValue = function (weekdayState, newAverage) {

        weekdayState.values.push(newAverage);
        weekdayState.total = withFourDigits(weekdayState.total + newAverage);
    };

    var substractRollingValueIfNeceessary = function (weekdayState, numberOfValues) {

        if (numberOfValues > rollingAverageNumbers) {
            weekdayState.total -= weekdayState.values.shift();
        }
    };

    var updateAverage = function (weekdayState, numberOfValues) {

        weekdayState.average = withFourDigits(weekdayState.total / numberOfValues);
    };

    var updateState = function (state, weekday, newAverage) {

        var weekdayState = state[weekday];

        addNewValue(weekdayState, newAverage);

        var numberOfValues = weekdayState.values.length;

        substractRollingValueIfNeceessary(weekdayState, numberOfValues);
        updateAverage(weekdayState, numberOfValues);
    };

    var handler = function (previousState, measurementEvent) {

        if (measurementEvent.body == null) return previousState;

        var state = ensureDefaultState(previousState);
        var weekday = getWeekDay(measurementEvent.body.Timeslot);

        updateState(state, weekday, measurementEvent.body.Average);

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
        MeasurementRead: deviceTypeRollingAveragePerWeekHour().handleEvent
    });