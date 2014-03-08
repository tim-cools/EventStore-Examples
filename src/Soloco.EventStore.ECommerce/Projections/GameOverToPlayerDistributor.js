/// <reference path="../References/jasmine/jasmine.js"/>
/// <reference path="../GameOverToPlayerDistributor.js"/>

describe("when distributing GameOver event to to players", function () {

    var distributor;
    var projections;

    beforeEach(function () {
        projections = jasmine.createSpyObj('projections', ['emit']);
        distributor = gameOverToPlayerDistributor(projections);
    });

    describe("when processing GameOver event", function () {

        describe("given amount is positive", function () {

            it("should emit a GameWon event to the player stream", function () {

                distributor.process({}, {
                    body: {
                        GameId: 'Game-abc',
                        Timestamp: '2014-02-20T08:02:39.687Z',
                        PlayerResults: [
                            { PlayerId: 'Player-p1', Amount: 100 }
                        ]
                    }
                });

                expect(projections.emit)
                   .wasCalledWith("Player-p1", "GameWon", {
                       PlayerId: 'Player-p1',
                       GameId: 'Game-abc',
                       Amount: 100,
                       Timestamp: '2014-02-20T08:02:39.687Z'
                   });
            });
        });

        describe("given amount is negative", function () {

            it("should emit a GameLost event to the player stream", function () {

                distributor.process({}, {
                    body: {
                        GameId: 'Game-abc',
                        Timestamp: '2014-02-20T08:02:39.687Z',
                        PlayerResults: [
                            { PlayerId: 'Player-p1', Amount: -100 }
                        ]
                    }
                });

                expect(projections.emit)
                    .wasCalledWith("Player-p1", "GameLost", {
                        PlayerId: 'Player-p1',
                        GameId: 'Game-abc',
                        Amount: -100,
                        Timestamp: '2014-02-20T08:02:39.687Z'
                    });
            });
        });

        describe("given multiple players", function () {

            beforeEach(function () {
                distributor.process({}, {
                    body: {
                        GameId: 'Game-abc',
                        Timestamp: '2014-02-20T08:02:39.687Z',
                        PlayerResults: [
                            { PlayerId: 'Player-p1', Amount: -100 },
                            { PlayerId: 'Player-p2', Amount: 80 },
                            { PlayerId: 'Player-p3', Amount: -40 },
                            { PlayerId: 'Player-p4', Amount: 20 }
                        ]
                    }
                });
            });

            it("should emit a GameWon event to the winning players stream", function () {

                expect(projections.emit)
                    .wasCalledWith("Player-p2", "GameWon", {
                        PlayerId: 'Player-p2',
                        GameId: 'Game-abc',
                        Amount: 80,
                        Timestamp: '2014-02-20T08:02:39.687Z',
                    });

                expect(projections.emit)
                    .wasCalledWith("Player-p4", "GameWon", {
                        PlayerId: 'Player-p4',
                        GameId: 'Game-abc',
                        Amount: 20,
                        Timestamp: '2014-02-20T08:02:39.687Z',
                    });
            });

            it("should emit a GameLost event to the losing players stream", function () {

                expect(projections.emit)
                    .wasCalledWith("Player-p1", "GameLost", {
                        PlayerId: 'Player-p1',
                        GameId: 'Game-abc',
                        Amount: -100,
                        Timestamp: '2014-02-20T08:02:39.687Z',
                    });

                expect(projections.emit)
                    .wasCalledWith("Player-p3", "GameLost", {
                        PlayerId: 'Player-p3',
                        GameId: 'Game-abc',
                        Amount: -40,
                        Timestamp: '2014-02-20T08:02:39.687Z',
                    });
            });
        });
    });
});