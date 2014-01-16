/// <reference path="../References/jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadAveragePerDayCalculator-Typed.js"/>

describe("when projecting measurement reads average per day (typed)", function () {
    
    var handler;
    var defaultEvent;
    var projections;

    beforeEach(function () {
        defaultEvent = { body: {} };
        projections = jasmine.createSpyObj('projections', ['emit']);
        handler = measurementReadAveragPerDayCalculator(projections);
    });

    describe("given state is empty", function () {
        
        it("should return initial state for first event", function() {

            var event = { body: { Timestamp: "2030-12-18T08:02:39.687Z", Reading: 1.23 } };
            var state = handler.init();
            
            var actcual = handler.handleEvent(state, event);

            expect(actcual).toEqual({
                total: 1.23,
                count: 1,
                lastTimestamp: "2030-12-18T08:02:39.687Z"
            });
        });
    });

    describe("given events are in the same timeslot", function () {

        it("should return calculate average for all events", function() {

            var state = handler.init();
            state = handler.handleEvent(state, { body: { Timestamp: "2030-12-18T08:00:39.687Z", Reading: 1.23 } });
            state = handler.handleEvent(state, { body: { Timestamp: "2030-12-18T09:01:39.687Z", Reading: 2.34 } });
            state = handler.handleEvent(state, { body: { Timestamp: "2030-12-18T10:02:39.687Z", Reading: 3.45 } });

            expect(state).toEqual({
                total: 7.02,
                count: 3,
                lastTimestamp: "2030-12-18T10:02:39.687Z"
            });
        });
    });

    describe("given events are in the different timeslot", function () {

        var state;
        
        beforeEach(function () {
            state = handler.init();
            state = handler.handleEvent(state, { streamId: "device1", body: { Timestamp: "2030-12-18T08:00:39.687Z", Reading: 1.23 } });
            state = handler.handleEvent(state, { streamId: "device1", body: { Timestamp: "2030-12-18T09:01:39.687Z", Reading: 2.34 } });
            state = handler.handleEvent(state, { streamId: "device1", body: { Timestamp: "2030-12-19T10:02:39.687Z", Reading: 3.44 } });
            state = handler.handleEvent(state, { streamId: "device1", body: { Timestamp: "2030-12-19T11:02:39.687Z", Reading: 4.56 } });
        });

        it("an event with calculated average of first timeframe should be emitted", function () {
            expect(projections.emit).toHaveBeenCalledWith(
                'MeasurementAverageDay-device1',
                'MeasurementAverageDay',
                { Timeslot: '2030/12/18', Total: 3.57, Count: 2, Average: 1.785 }
            );
        });
        
        it("should return calculate average for events of new timeframe", function () {

            expect(state).toEqual({
                total: 8,
                count: 2,
                lastTimestamp: "2030-12-19T11:02:39.687Z"
            });
        });
    });
});