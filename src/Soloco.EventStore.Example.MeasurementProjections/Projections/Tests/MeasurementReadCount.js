/// <reference path="jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadCount.js"/>

describe("when counting measurement reads", function () {
    
    var handler;
    var defaultEvent;

    beforeEach(function () {
        defaultEvent = { body: {} };
        handler = measurementReadCount(projections);
    });

    it("should initialize new state on first event", function () {

        var initState = handler.handleEvent({}, defaultEvent);

        expect(initState.count).toEqual(1);
    });

    it("should count the number of events", function () {

        var state = handler.handleEvent({}, defaultEvent);
        state = handler.handleEvent(state, defaultEvent);
        state = handler.handleEvent(state, defaultEvent);

        expect(state.count).toEqual(3);
    });
});