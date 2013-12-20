// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

var measurementReadAveragPerDay = function measurementReadAveragPerDayConstuctor($eventServices) {
    
    var eventServices = !$eventServices ? { emit: emit } : $eventServices;
    
    var zeroPad = function(num, places) {
        var zero = places - num.toString().length + 1;
        return Array(+(zero > 0 && zero)).join("0") + num;
    };

    var formatTimeSlot = function(timestamp) {
        var date = new Date(timestamp);
        return date.getFullYear().toString() + '/' +
            zeroPad(date.getMonth() + 1, 2) + '/' +
            zeroPad(date.getDate(), 2);
    };

    var isDifferentTimeSlot = function(currentTimestamp, lastTimestamp) {
        return formatTimeSlot(currentTimestamp) != formatTimeSlot(lastTimestamp);
    };

    var eventStreamId = function(originalStreamId) {
        return 'MeasurementAverageDay-' + originalStreamId;
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

    var emitAverageEvent = function (measurementEvent, previousState) {
        
        var name = eventStreamId(measurementEvent.streamId, previousState.lastTimestamp);
        var event = createEvent(formatTimeSlot(previousState.lastTimestamp), previousState.total, previousState.count);

        eventServices.emit(name, "MeasurementAverageDay", event);
    };
    
    var handleEvent = function (previousState, measurementEvent) {
     
        if (!measurementEvent.body) return previousState;

        var timestamp = measurementEvent.body.Timestamp;
        var reading = measurementEvent.body.Reading;

        if (!previousState.total) {
            return createState(reading, 1, timestamp);
        }

        if (isDifferentTimeSlot(timestamp, previousState.lastTimestamp)) {

            emitAverageEvent(measurementEvent, previousState);
            
            return createState(reading, 1, timestamp);
        } else {
            return createState(previousState.total + reading, previousState.count + 1, timestamp);
        }
    };

    return {
        formatTimeSlot: formatTimeSlot,
        handleEvent: handleEvent
    };
};

fromCategory('Meter')
    .foreachStream()
    .when( {
        MeasurementRead: measurementReadAveragPerDay().handleEvent
    });