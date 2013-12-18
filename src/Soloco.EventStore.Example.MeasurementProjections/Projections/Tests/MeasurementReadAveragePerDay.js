/// <reference path="jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadAveragePerDay.js"/>

describe("when projecting measurement reads average per day", function () {
    
    var handler;
    var defaultEvent;
    var projections;

    beforeEach(function () {
        defaultEvent = { body: {} };
        projections = jasmine.createSpyObj('projections', ['emit']);
        handler = measurementReadAveragPerDay(projections);
    });

    describe("when formatting the timeframe", function () {
        
        beforeEach(function () {

            this.addMatchers({
                
                toMatchTimeSlot: function (expected) {

                    this.actual = handler.formatTimeSlot(this.actual);
                    this.message = function () { return "Expected TimeSlot of '" + this.actual + "' to be '" + expected + "'"; };
                    
                    return (this.actual == expected);
                }
            });
        });
    
        it("should contain year month and date", function () {

            expect("2000-01-01T08:02:39.687Z").toMatchTimeSlot("2000/01/01");
            expect("2005-06-24T08:02:39.687Z").toMatchTimeSlot("2005/06/24");
            expect("2012-10-10T08:02:39.687Z").toMatchTimeSlot("2012/10/10");
            expect("2015-11-12T08:02:39.687Z").toMatchTimeSlot("2015/11/12");
            expect("2030-12-18T08:02:39.687Z").toMatchTimeSlot("2030/12/18");

        });
    });

    it("should return current state when body is null", function () {
        
        var previousState = { state: 'original' };
        var emptyEvent = { body: null };
        
        var actcual = handler.handleEvent(previousState, emptyEvent);

        expect(actcual).toEqual(previousState);
    });

    describe("given state is empty", function () {
        
        it("should return initial state for first event", function() {

            var event = { body: { Timestamp: "2030-12-18T08:02:39.687Z", Reading: 1.23 } };
            var state = {};
            
            var actcual = handler.handleEvent(state, event);

            expect(actcual).toEqual({
                total: 1.23,
                count: 1,
                average: 1.23,
                lastTimestamp: "2030-12-18T08:02:39.687Z"
            });
        });
    });

    describe("given events are in the same timeslot", function () {

        it("should return calculate average for all events", function() {

            var state = {};
            state = handler.handleEvent(state, { body: { Timestamp: "2030-12-18T08:00:39.687Z", Reading: 1.23 } });
            state = handler.handleEvent(state, { body: { Timestamp: "2030-12-18T09:01:39.687Z", Reading: 2.34 } });
            state = handler.handleEvent(state, { body: { Timestamp: "2030-12-18T10:02:39.687Z", Reading: 3.45 } });

            expect(state).toEqual({
                total: 7.02,
                count: 3,
                average: 2.34,
                lastTimestamp: "2030-12-18T10:02:39.687Z"
            });
        });
    });

    describe("given events are in the different timeslot", function () {

        var state;
        
        beforeEach(function () {
            state = {};
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
                average: 4,
                lastTimestamp: "2030-12-19T11:02:39.687Z"
            });
        });
    });
});