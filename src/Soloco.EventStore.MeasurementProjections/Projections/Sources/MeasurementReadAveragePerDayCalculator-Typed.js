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

var mixin = function mixinConstructr() {

    var add = function (destination, mixin) {
        for (var k in mixin) {
            if (mixin.hasOwnProperty(k)) {
                destination[k] = mixin[k];
            }
        }
        return destination;
    };

    var remove = function (destination, mixin) {
        for (var k in mixin) {
            if (mixin.hasOwnProperty(k)) {
                delete destination[k];
            }
        }
        return destination;
    };
    
    return {
        add: add,
        remove: remove,
    };
}();

var measurementReadAveragPerDayCalculator = function measurementReadAveragPerDayCalculatorConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;   

    var stateMixin = {

        getAverage: function () {
            return this.total / this.count;
        },
        isFirstTime: function () {
            return !this.lastTimestamp;
        },
        isSameTimeSlot: function (timestamp) {
            return dailyTimestampFormatter.format(timestamp)
                == dailyTimestampFormatter.format(this.lastTimestamp);
        },
        update: function (reading, timestamp) {

            if (this.isFirstTime() || this.isSameTimeSlot(timestamp)) {
                return createState({ total: this.total + reading, count: this.count + 1, lastTimestamp: timestamp });
            }

            return createState({ total: reading, count: 1, lastTimestamp: timestamp });
        },
        isInDifferentTimeSlotAs: function (previousState) {
            return !previousState.isFirstTime() && !this.isSameTimeSlot(previousState.timeStamp);
        },
        createEvent: function () {
            return {
                Timeslot: dailyTimestampFormatter.format(this.lastTimestamp),
                Total: this.total,
                Count: this.count,
                Average: this.getAverage()
            };
        },
        clean: function () {
            return mixin.remove(this, stateMixin);
        }
    };
    
    var createState = function (state) {
        if (state == null) { state = { total: 0, count: 0, lastTimestamp: null }; }
        
        return mixin.add(state, stateMixin);
    };
    
    var emitAverageEvent = function (measurementEvent, state) {

        eventServices.emit(
            'MeasurementAverageDay-' + measurementEvent.streamId, 
            'MeasurementAverageDay', 
            state.createEvent());
    };

    var handleEvent = function (previousState, measurementEvent) {

        var timestamp = measurementEvent.body.Timestamp;
        var reading = measurementEvent.body.Reading;
        
        var state = createState(previousState);
        var updatedState = state.update(reading, timestamp);
        
        if (updatedState.isInDifferentTimeSlotAs(state)) {
            emitAverageEvent(measurementEvent, state);
        }
        return updatedState.clean();
    };

    return {
        init: createState,
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