/// <reference path="../References/jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadAveragePerDayCalculator.js"/>

describe("when formatting the timeframe", function () {

    var formattedTimeSlot = function(value) {
        return dailyTimestampFormatter.format(value);
    };
    
    it("should contain year month and date", function () {

        expect(formattedTimeSlot("2000-01-01T08:02:39.687Z")).toMatch("2000/01/01");
        expect(formattedTimeSlot("2005-06-24T08:02:39.687Z")).toMatch("2005/06/24");
        expect(formattedTimeSlot("2012-10-10T08:02:39.687Z")).toMatch("2012/10/10");
        expect(formattedTimeSlot("2015-11-12T08:02:39.687Z")).toMatch("2015/11/12");
        expect(formattedTimeSlot("2030-12-18T08:02:39.687Z")).toMatch("2030/12/18");
    });
});

describe("when projecting measurement reads average per day", function () {
    
    var handler;
    var defaultEvent;
    var projections;

    var eventEnvelope = function(timestamp, reading) {
        return { streamId: "device1", body: { Timestamp: timestamp, Reading: reading } };
    };

    beforeEach(function () {
        defaultEvent = { body: {} };
        projections = jasmine.createSpyObj('projections', ['emit']);
        handler = measurementReadAveragPerDayCalculator(projections);
    });

    describe("given state is empty", function () {
        
        it("should return initial state for first event", function() {

            var event = { body: { Timestamp: "2030-12-18T08:02:39.687Z", Reading: 1.23 } };
            var state = handler.init();
            
            var actcual = handler.update(state, event);

            expect(actcual).toEqual({
                total: 1.23,
                count: 1,
                average: 1.23,
                lastTimestamp: "2030-12-18T08:02:39.687Z"
            });
        });
    });

    describe("given events are in the same timeslot", function () {

        it("should return calculated average for all events", function() {

            var state = handler.init();
            state = handler.update(state, eventEnvelope("2030-12-18T08:00:39.687Z", 1.23));
            state = handler.update(state, eventEnvelope("2030-12-18T09:01:39.687Z", 2.34));
            state = handler.update(state, eventEnvelope("2030-12-18T10:02:39.687Z", 3.45));

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
            state = handler.init();
            state = handler.update(state, eventEnvelope("2030-12-18T08:00:39.687Z", 1.23));
            state = handler.update(state, eventEnvelope("2030-12-18T09:01:39.687Z", 2.34));
            state = handler.update(state, eventEnvelope("2030-12-19T10:02:39.687Z", 3.44));
            state = handler.update(state, eventEnvelope("2030-12-19T11:02:39.687Z", 4.56));
        });

        it("an event with calculated average of first timeframe should be emitted", function () {
            expect(projections.emit).toHaveBeenCalledWith(
                'MeasurementAverageDay-device1',
                'MeasurementAverageDay',
                { Timeslot: '2030/12/18', Total: 3.57, Count: 2, Average: 1.785 }
            );
        });
        
        it("should return calculated average for events of new timeframe", function () {

            expect(state).toEqual({
                total: 8,
                count: 2,
                average: 4,
                lastTimestamp: "2030-12-19T11:02:39.687Z"
            });
        });
    });
});