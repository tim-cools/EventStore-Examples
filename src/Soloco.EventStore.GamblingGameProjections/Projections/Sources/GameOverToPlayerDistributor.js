// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

var gameOverToPlayerDistributor = function gameOverToPlayerDistributorConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var createEvent = function (playerId, gameId, amount, timestamp) {
        return {
            PlayerId: playerId,
            GameId: gameId,
            Amount: amount,
            Timestamp: timestamp
        };
    };

    var emitGameEvent = function (eventType, gameId, playerId, amount, timestamp) {

        var event = createEvent(playerId, gameId, amount, timestamp);

        eventServices.emit(playerId, eventType, event);
    };

    var processPlayer = function (gameId, playerResult, timestamp) {
        if (playerResult.Amount > 0) {
            emitGameEvent("GameWon", gameId, playerResult.PlayerId, playerResult.Amount, timestamp);
        } else if (playerResult.Amount < 0) {
            emitGameEvent("GameLost", gameId, playerResult.PlayerId, playerResult.Amount, timestamp);
        }
    };
    
    var process = function (state, event) {

        var gameId = event.body.GameId;
        var players = event.body.PlayerResults;
        var timestamp = event.body.Timestamp;

        for (var playerIndex = 0; playerIndex < players.length; playerIndex++) {
            var player = players[playerIndex];
            processPlayer(gameId, player, timestamp);
        }
    };

    return {
        process: process
    };
};

var distributor = gameOverToPlayerDistributor();

fromCategory('Game')
    .when({
        GameOver: distributor.process
    });