// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

var dailyTimestampFormatter = function dailyTimestampFormatterConstructr() {

    var zeroPad = function (num, places) {
        var zero = places - num.toString().length + 1;
        return Array(+(zero > 0 && zero)).join("0") + num;
    };

    var format = function (timestamp) {
        var date = new Date(timestamp);
        return date.getFullYear().toString() + '/' +
            zeroPad(date.getMonth() + 1, 2) + '/' +
            zeroPad(date.getDate(), 2);
    };
    
    return {
        format: format
    };
}();

var measurementReadAveragPerDayCalculator = function measurementReadAveragPerDayCalculatorConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var isFirstTime = function(previousState) {
         return !previousState.lastTimestamp;
    };

    var isSameTimeSlot = function (currentTimestamp, lastTimestamp) {
        return dailyTimestampFormatter.format(currentTimestamp)
            == dailyTimestampFormatter.format(lastTimestamp);
    };

    var createEvent = function (timeslot, reading, count) {
        return {
            Timeslot: timeslot,
            Total: reading,
            Count: count,
            Average: reading / count
        };
    };

    var createState = function (reading, count, timestamp) {
        return {
            total: reading,
            count: count,
            average: reading / count,
            lastTimestamp: timestamp
        };
    };

    var emitAverageEvent = function (measurementEvent, previousState) {

        var streamName = 'MeasurementAverageDay-' + measurementEvent.streamId;
        var event = createEvent(dailyTimestampFormatter.format(previousState.lastTimestamp), previousState.total, previousState.count);

        eventServices.emit(streamName, "MeasurementAverageDay", event);
    };

    var init = function () {
        return createState(0, 0, null);
    };
    
    var handleEvent = function (previousState, measurementEvent) {

        var timestamp = measurementEvent.body.Timestamp;
        var reading = measurementEvent.body.Reading;

        if (isFirstTime(previousState) || isSameTimeSlot(timestamp, previousState.lastTimestamp)) {

            return createState(previousState.total + reading, previousState.count + 1, timestamp);
        }
        else
        {
            emitAverageEvent(measurementEvent, previousState);

            return createState(reading, 1, timestamp);
        }
    };

    return {
        init: init,
        handleEvent: handleEvent
    };
};

var calculator = measurementReadAveragPerDayCalculator();

fromCategory('Device')
    .foreachStream()
    .when({
        $init: calculator.init,
        MeasurementRead: calculator.handleEvent
    });