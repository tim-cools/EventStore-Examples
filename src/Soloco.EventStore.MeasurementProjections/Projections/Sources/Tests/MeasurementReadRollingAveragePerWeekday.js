/// <reference path="jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadRollingAveragePerWeekday.js"/>

describe("when projecting measurement reads for a rolling average per week day", function () {

    var handler;
    var defaultEvent;
    var projections;

    var handleEvent = function (state, timeslot, average) {
        
        var event = { streamId: "device1", body: { "Timeslot": timeslot, "Average": average } };
        return handler.handleEvent(state, event);
    };

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
                Monday: { values: [], average: 0, total: 0 },
                Tuesday: { values: [], average: 0, total: 0 },
                Wednesday: { values: [21.625], average: 21.625, total: 21.625 },
                Thursday: { values: [], average: 0, total: 0 },
                Friday: { values: [], average: 0, total: 0 },
                Saturday: { values: [], average: 0, total: 0 },
                Sunday: { values: [], average: 0, total: 0 }
            });
        });
    });

    describe("given number of events lower than the threshold with the same weekday are handled", function () {

        var state;

        beforeEach(function () {
            state = {};
            state = handleEvent(state, "2013/12/18", 21.625);
            state = handleEvent(state, "2013/12/25", 18.452);
            state = handleEvent(state, "2014/01/01", 19.123);
            state = handleEvent(state, "2014/01/08", 16.873);
            state = handleEvent(state, "2014/01/15", 21.962);
            state = handleEvent(state, "2014/01/22", 20.351);
        });
        
        it("should the return the state with the average of the all timeslots", function () {
            
            expect(state).toEqual({
                Monday: { values: [], average: 0, total: 0 },
                Tuesday: { values: [], average: 0, total: 0 },
                Wednesday: {
                    values: [21.625, 18.452, 19.123, 16.873, 21.962, 20.351],
                    average: 19.731,
                    total: 118.386
                },
                Thursday: { values: [], average: 0, total: 0 },
                Friday: { values: [], average: 0, total: 0 },
                Saturday: { values: [], average: 0, total: 0 },
                Sunday: { values: [], average: 0, total: 0 }
            });
        });

        it("should emit a first event with first value", function () {
            
            expect(projections.emit).toHaveBeenCalledWith(
                'MeasurementRollingAveragePerWeekday-device1',
                'MeasurementRollingAveragePerWeekday',
                {
                    Monday: 0,
                    Tuesday: 0,
                    Wednesday: 21.625,
                    Thursday: 0,
                    Friday: 0,
                    Saturday: 0,
                    Sunday: 0,
                }
            );
        });

        it("should emit an event for each ", function () {
            
            expect(projections.emit.callCount).toBe(6);
        });
        
        it("should emit a last event with calculated averages", function () {
            
            expect(projections.emit).toHaveBeenCalledWith(
                'MeasurementRollingAveragePerWeekday-device1',
                'MeasurementRollingAveragePerWeekday',
                {
                    Monday: 0,
                    Tuesday: 0,
                    Wednesday: 19.731,
                    Thursday: 0,
                    Friday: 0,
                    Saturday: 0,
                    Sunday: 0,
                }
            );
        });
    });

    describe("given a number of events that exceeds the threshold with the same weekday are handled", function () {

        var state;

        beforeEach(function () {
            state = {};
            state = handleEvent(state, "2013/12/18", 21.625);
            state = handleEvent(state, "2013/12/25", 18.452);
            state = handleEvent(state, "2014/01/01", 19.123);
            state = handleEvent(state, "2014/01/08", 16.873);
            state = handleEvent(state, "2014/01/15", 21.962);
            state = handleEvent(state, "2014/01/22", 20.351);
            state = handleEvent(state, "2014/01/29", 18.324);
            state = handleEvent(state, "2014/02/05", 19.287);
            state = handleEvent(state, "2014/02/12", 21.876);
            state = handleEvent(state, "2014/02/19", 22.943);
        });
        
        it("should the return the state with the average of the last 7 timeslots", function () {

            expect(state).toEqual({
                Monday: { values: [], average: 0, total: 0 },
                Tuesday: { values: [], average: 0, total: 0 },
                Wednesday: {
                    values: [16.873, 21.962, 20.351, 18.324, 19.287, 21.876, 22.943],
                    average: 17.702,
                    total: 141.616
                },
                Thursday: { values: [], average: 0, total: 0 },
                Friday: { values: [], average: 0, total: 0 },
                Saturday: { values: [], average: 0, total: 0 },
                Sunday: { values: [], average: 0, total: 0 }
            });
        });

        it("should emit an event with calculated averages", function () {
            expect(projections.emit).toHaveBeenCalledWith(
                'MeasurementRollingAveragePerWeekday-device1',
                'MeasurementRollingAveragePerWeekday',
                {
                    Monday: 0,
                    Tuesday: 0,
                    Wednesday: 17.702,
                    Thursday: 0,
                    Friday: 0,
                    Saturday: 0,
                    Sunday: 0,
                }
            );
        });
    });


    describe("given an event is handled for each weekday", function () {

        var state;

        beforeEach(function () {
            state = {};
            state = handleEvent(state, "2013/12/18", 20.351);
            state = handleEvent(state, "2013/12/19", 18.324);
            state = handleEvent(state, "2013/12/20", 19.287);
            state = handleEvent(state, "2013/12/21", 21.876);
            state = handleEvent(state, "2013/12/22", 22.943);
            state = handleEvent(state, "2013/12/23", 16.873);
            state = handleEvent(state, "2013/12/24", 21.962);
        });

        it("should the return the state with the average for each day", function () {

            expect(state).toEqual({
                Monday:    { values: [16.873], average: 16.873, total: 16.873 },
                Tuesday:   { values: [21.962], average: 21.962, total: 21.962 },
                Wednesday: { values: [20.351], average: 20.351, total: 20.351 },
                Thursday:  { values: [18.324], average: 18.324, total: 18.324 },
                Friday:    { values: [19.287], average: 19.287, total: 19.287 },
                Saturday:  { values: [21.876], average: 21.876, total: 21.876 },
                Sunday:    { values: [22.943], average: 22.943, total: 22.943 },
            });
        });

        it("should emit an event with an averages for each day", function () {

            expect(projections.emit).toHaveBeenCalledWith(
                'MeasurementRollingAveragePerWeekday-device1',
                'MeasurementRollingAveragePerWeekday',
                {
                    Monday: 16.873,
                    Tuesday: 21.962,
                    Wednesday: 20.351,
                    Thursday: 18.324,
                    Friday: 19.287,
                    Saturday: 21.876,
                    Sunday: 22.943
                }
            );
        });
    });
});