/// <reference path="jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadRollingAveragePerWeekday.js"/>

describe("when projecting measurement reads for a rolling average per week day", function () {

    var handler;
    var defaultEvent;
    var projections;

    beforeEach(function () {
        defaultEvent = { body: {} };
        projections = jasmine.createSpyObj('projections', ['emit']);
        handler = measurementReadRollingAveragPerWeekDay(projections);
    });

    describe("when getting the day of the week", function () {

        beforeEach(function () {

            this.addMatchers({

                toMatchDayOfWeek: function (expected) {

                    this.actual = handler.getWeekDay(this.actual);
                    this.message = function () { return "Expected WeekDay of '" + this.actual + "' to be '" + expected + "'"; };

                    return (this.actual == expected);
                }
            });
        });

        it("should return the correct day of the week", function () {

            expect("2000-01-01T08:02:39.687Z").toMatchDayOfWeek("Saturday");
            expect("2005-06-24T08:02:39.687Z").toMatchDayOfWeek("Friday");
            expect("2013-12-18T08:02:39.687Z").toMatchDayOfWeek("Wednesday");
            expect("2015-11-12T08:02:39.687Z").toMatchDayOfWeek("Thursday");
            expect("2030-12-18T08:02:39.687Z").toMatchDayOfWeek("Wednesday");

        });
    });

    it("should return current state when body is null", function () {

        var previousState = { state: 'original' };
        var emptyEvent = { body: null };

        var actcual = handler.handleEvent(previousState, emptyEvent);

        expect(actcual).toEqual(previousState);
    });

    describe("given state is empty", function () {

        it("should return initial state for first event", function () {

            var event = {
                body: {
                    "Timeslot": "2013/12/18",
                    "Average": 21.625
                }
            };
            var state = {};

            var actcual = handler.handleEvent(state, event);

            expect(actcual).toEqual({
                Wednesday: {
                    values: [21.625],
                    average: 21.625,
                    total: 21.625
                }
            });
        });
    });

    describe("given number of events lower than the threshold with the same weekday are handled", function () {

        var state;

        beforeEach(function () {
            state = {};
            state = handler.handleEvent(state, { body: { "Timeslot": "2013/12/18", "Average": 21.625 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2013/12/25", "Average": 18.452 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/01", "Average": 19.123 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/08", "Average": 16.873 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/15", "Average": 21.962 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/22", "Average": 20.351 } });
        });

        it("should the return the state with the average of the all timeslots", function () {
            
            expect(state).toEqual({
                Wednesday: {
                    values: [21.625, 18.452, 19.123, 16.873, 21.962, 20.351],
                    average: 19.731,
                    total: 118.386
                }
            });
        });

        //    it("an event with calculated average of first timeframe should be emitted", function () {
        //        expect(projections.emit).toHaveBeenCalledWith(
        //            'MeasurementPeriod-device1+D',
        //            'MeasurementPeriod',
        //            { Timeslot: 'D2030/12/18', Total: 3.57, Count: 2, Average: 1.785 }
        //        );
        //    });
    });


    describe("given a number of events that exceeds the threshold with the same weekday are handled", function () {

        var state;

        beforeEach(function () {
            state = {};
            state = handler.handleEvent(state, { body: { "Timeslot": "2013/12/18", "Average": 21.625 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2013/12/25", "Average": 18.452 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/01", "Average": 19.123 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/08", "Average": 16.873 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/15", "Average": 21.962 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/22", "Average": 20.351 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/01/29", "Average": 18.324 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/02/05", "Average": 19.287 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/02/12", "Average": 21.876 } });
            state = handler.handleEvent(state, { body: { "Timeslot": "2014/02/19", "Average": 22.943 } });
        });

        it("should the return the state with the average of the last 7 timeslots", function () {

            expect(state).toEqual({
                Wednesday: {
                    values: [16.873, 21.962, 20.351, 18.324, 19.287, 21.876, 22.943],
                    average: 17.702,
                    total: 141.616
                }
            });
        });

        //    it("an event with calculated average of first timeframe should be emitted", function () {
        //        expect(projections.emit).toHaveBeenCalledWith(
        //            'MeasurementPeriod-device1+D',
        //            'MeasurementPeriod',
        //            { Timeslot: 'D2030/12/18', Total: 3.57, Count: 2, Average: 1.785 }
        //        );
        //    });
    });
});