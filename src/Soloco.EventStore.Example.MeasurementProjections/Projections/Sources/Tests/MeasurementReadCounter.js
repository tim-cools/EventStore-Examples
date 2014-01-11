/// <reference path="../References/jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadCounter.js"/>

describe("when counting measurement reads", function () {
    
    var defaultEvent = { body: {} };
    var counter;

    beforeEach(function () {
        counter = eventCounter();
    });

    it("should initialize new state with count 0", function () {

        var initState = counter.init();

        expect(initState.count).toEqual(0);
    });

    it("should count the number of events", function () {

        var state = counter.init();
        state = counter.increase(state, defaultEvent);
        state = counter.increase(state, defaultEvent);
        state = counter.increase(state, defaultEvent);

        expect(state.count).toEqual(3);
    });
});