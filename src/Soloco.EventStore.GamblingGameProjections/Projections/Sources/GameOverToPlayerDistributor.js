// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

var gameOverToPlayerDistributor = function gameOverToPlayerDistributorConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var createEvent = function (playerId, gameId, amount) {
        return {
            PlayerId: playerId,
            GameId: gameId,
            Amount: amount,
        };
    };

    var emitGameEvent = function (eventType, gameId, playerId, amount) {

        var event = createEvent(playerId, gameId, amount);

        eventServices.emit(playerId, eventType, event);
    };

    var processPlayer = function (gameId, playerResult) {
        if (playerResult.Amount > 0) {
            emitGameEvent("GameWon", gameId, playerResult.PlayerId, playerResult.Amount);
        } else if (playerResult.Amount < 0) {
            emitGameEvent("GameLost", gameId, playerResult.PlayerId, playerResult.Amount);
        }
    };
    
    var process = function (previousState, measurementEvent) {

        var gameId = measurementEvent.body.GameId;
        var players = measurementEvent.body.PlayerResults;

        for (var playerIndex = 0; playerIndex < players.length; playerIndex++) {
            var player = players[playerIndex];
            processPlayer(gameId, player);
        }
        
        return previousState;
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