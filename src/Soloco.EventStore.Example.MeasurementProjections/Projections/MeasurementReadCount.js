/// <reference path="References\1Prelude.js" />

var eventCounter = function () {

    var init = function () {
        return { count: 0 };
    };
     
    var countEvents = function (state, eventEnvelope) {

        state.count += 1;

        return state;
    };

    return {
        init: init,
        increase: countEvents
    };
};

var counter = eventCounter();

fromAll()
    .when({
        $init: counter.init,
        MeasurementRead: counter.increase
    });