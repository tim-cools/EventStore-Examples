/// <reference path="../References/jasmine/jasmine.js" />
/// <reference path="../IrresponsibleGamblingDetector.js"/>

describe("when detecting irresponsible gamblers", function () {

    var detector;
    var projections;

    beforeEach(function () {
        projections = jasmine.createSpyObj('projections', ['emit']);
        detector = irresponsibleGamblingDetector(projections);
    });

    describe("when processing GameLost event", function () {

        describe("given amount lost is less than 500", function () {

            var state;

            beforeEach(function () {

                state = detector.init();
                state = detector.processGameLost(state, {
                    sequenceNumber: 10,
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -100, Timestamp: '2013-12-18T08:02:39.687Z' }
                });
            });
            
            it("state should contain the one amount", function () {

                expect(state).toEqual({
                    LastAlarm: null,
                    GamesLast24Hour: [
                        { Timestamp: '2013-12-18T08:02:39.687Z', Amount: -100, SequenceNumber: 10 }
                    ]
                });
            });

            it("no event should be emitted", function () {

                expect(projections.emit).wasNotCalled();
            });
        });

        describe("given amount lost is greather than 500 in last 24 hours", function () {

            var state;

            beforeEach(function () {

                state = detector.init();
                state = detector.processGameLost(state, {
                    sequenceNumber: 1,
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -100, Timestamp: '2013-12-18T08:02:39.687Z' }
                });

                state = detector.processGameLost(state, {
                    sequenceNumber: 2,
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -401, Timestamp: '2013-12-19T08:02:39.687Z' }
                });
            });

            it("state should contain the both amounts", function () {

                expect(state).toEqual({
                    LastAlarm: '2013-12-19T08:02:39.687Z',
                    GamesLast24Hour: [
                        { Timestamp: '2013-12-18T08:02:39.687Z', Amount: -100, SequenceNumber: 1 },
                        { Timestamp: '2013-12-19T08:02:39.687Z', Amount: -401, SequenceNumber: 2 }
                    ]
                });
            });

            it("should emit an alarm", function () {

                expect(projections.emit).wasCalledWith("IrresponsibleGamblingAlarms", "IrresponsibleGamblerDetected",
                    { PlayerId: 'Player-1', AmountSpendLAst24Hours: -501, Timestamp: '2013-12-19T08:02:39.687Z' }
                );
            });
        });


        describe("given amount lost is greather than 500 but not in the last 24 hours", function () {

            var state;

            beforeEach(function () {

                state = detector.init();
                state = detector.processGameLost(state, {
                    sequenceNumber: 1,
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -100, Timestamp: '2013-12-18T08:02:39.687Z' }
                });

                state = detector.processGameLost(state, {
                    sequenceNumber: 2,
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -401, Timestamp: '2013-12-19T09:02:39.687Z' }
                });
            });

            it("state should contain the both amounts", function () {

                expect(state).toEqual({
                    LastAlarm: null,
                    GamesLast24Hour: [
                        { Timestamp: '2013-12-19T09:02:39.687Z', Amount: -401, SequenceNumber: 2 }
                    ]
                });
            });

            it("should not emit an alarm", function () {

                expect(projections.emit).wasNotCalled();
            });
        });

        describe("given an event has already been processed", function () {

            var state;

            beforeEach(function () {

                var event = {
                    sequenceNumber: 10,
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -100, Timestamp: '2013-12-18T08:02:39.687Z' }
                };
                
                state = detector.init();
                state = detector.processGameLost(state, event);
                state = detector.processGameLost(state, event);
                state = detector.processGameLost(state, event);
            });

            it("state should contain the only one event", function () {

                expect(state).toEqual({
                    LastAlarm: null,
                    GamesLast24Hour: [
                        { Timestamp: '2013-12-18T08:02:39.687Z', Amount: -100, SequenceNumber: 10 }
                    ]
                });
            });
        });
    });
});