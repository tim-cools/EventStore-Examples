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
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -100, Timestamp: '2013-12-18T08:02:39.687Z' }
                });

                state = detector.processGameLost(state, {
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -401, Timestamp: '2013-12-19T08:02:39.687Z' }
                });
            });

            it("state should contain the both amounts", function () {

                expect(state).toEqual({
                    LastAlarm: '2013-12-19T08:02:39.687Z',
                    GamesLast24Hour: [
                        { Timestamp: '2013-12-18T08:02:39.687Z', Amount: -100 },
                        { Timestamp: '2013-12-19T08:02:39.687Z', Amount: -401 }
                    ]
                });
            });

            it("should emit an alarm", function () {

                expect(projections.emit).wasCalledWith("IrresponsibleGamblingAlarms", "IrresponsibleGamblerDetected",
                    { PlayerId: 'Player-1', TotalAmount: -501 }
                );
            });
        });


        describe("given amount lost is greather than 500 but not in the last 24 hours", function () {

            var state;

            beforeEach(function () {

                state = detector.init();
                state = detector.processGameLost(state, {
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -100, Timestamp: '2013-12-18T08:02:39.687Z' }
                });

                state = detector.processGameLost(state, {
                    body: { PlayerId: 'Player-1', GameId: 'Game-1', Amount: -401, Timestamp: '2013-13-19T08:02:39.687Z' }
                });
            });

            it("state should contain the both amounts", function () {

                expect(state).toEqual({
                    LastAlarm: null,
                    GamesLast24Hour: [
                        { Timestamp: '2013-12-18T08:02:39.687Z', Amount: -100 },
                        { Timestamp: '2013-13-19T08:02:39.687Z', Amount: -401 }
                    ]
                });
            });

            it("should not emit an alarm", function () {

                expect(projections.emit).wasNotCalled();
            });
        });

        //describe("given amount is negative", function() {

        //    it("should emit a GameLost event to the player stream", function() {

        //        distributor.process({}, {
        //            body: {
        //                GameId: 'Game-abc',
        //                Timestamp: '',
        //                PlayerResults: [
        //                    { PlayerId: 'Player-p1', Amount: -100 }
        //                ]
        //            }
        //        });

        //        expect(projections.emit)
        //            .wasCalledWith("Player-p1", "GameLost",
        //                { PlayerId: 'Player-p1', GameId: 'Game-abc', Amount: -100 });
        //    });
        //});


        //describe("given multiple players", function () {

        //    beforeEach(function() {
        //        distributor.process({}, {
        //            body: {
        //                GameId: 'Game-abc',
        //                Timestamp: '',
        //                PlayerResults: [
        //                    { PlayerId: 'Player-p1', Amount: -100 },
        //                    { PlayerId: 'Player-p2', Amount:   80 },
        //                    { PlayerId: 'Player-p3', Amount:  -40 },
        //                    { PlayerId: 'Player-p4', Amount:   20 }
        //                ]
        //            }
        //        });
        //    });

        //    it("should emit a GameWon event to the winning players stream", function () {

        //        expect(projections.emit)
        //            .wasCalledWith("Player-p2", "GameWon",
        //                { PlayerId: 'Player-p2', GameId: 'Game-abc', Amount: 80 });

        //        expect(projections.emit)
        //            .wasCalledWith("Player-p4", "GameWon",
        //                { PlayerId: 'Player-p4', GameId: 'Game-abc', Amount: 20 });
        //    });

        //    it("should emit a GameLost event to the losing players stream", function () {

        //        expect(projections.emit)
        //            .wasCalledWith("Player-p1", "GameLost",
        //                { PlayerId: 'Player-p1', GameId: 'Game-abc', Amount: -100 });

        //        expect(projections.emit)
        //            .wasCalledWith("Player-p3", "GameLost",
        //                { PlayerId: 'Player-p3', GameId: 'Game-abc', Amount: -40 });
        //    });
    });
});