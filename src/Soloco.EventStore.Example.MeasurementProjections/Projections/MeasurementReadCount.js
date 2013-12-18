/// <reference path="References\1Prelude.js" />

var measurementReadCount = function () {
    
    var handleEvent = function (s, e) {

        if (e.body == null) return s;

        s.count = s.count == null ? 1 : s.count + 1;

        return s;
    };

    return {
        handleEvent: handleEvent
    };
};

fromAll().when({
    MeasurementRead: measurementReadCount().handleEvent
});