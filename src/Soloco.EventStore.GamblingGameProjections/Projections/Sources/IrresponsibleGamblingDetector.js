// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

var irresponsibleGamblingDetector = function irresponsibleGamblingDetectorConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var millisecondsPerDay = 24 * 60 * 60 * 1000;
    var amountLostThreshold = 500;
    
    var emitAlarm = function (state, playerId, total, timestamp)  {

        state.LastAlarm = timestamp;

        eventServices.emit('IrresponsibleGamblingAlarms', 'IrresponsibleGamblerDetected', {
            PlayerId: playerId,
            AmountSpentLAst24Hours: total,
            Timestamp: timestamp
        });
    };

    var init = function () {
        return {            
            LastAlarm: null,
            GamesLast24Hour: [],
        };
    };

    var isMoreAs24HoursDifference = function (timestamp, reference) {

        if (reference === null) return true;

        var millisecondsDifference = new Date(timestamp).getTime() - new Date(reference).getTime();

        return millisecondsDifference > millisecondsPerDay  ;
    };

    var removeGamesOlderThan24HoursFromCache = function (gamesResults, timestamp) {

        while (gamesResults.length > 0 && isMoreAs24HoursDifference(timestamp, gamesResults[0].Timestamp)) {
            gamesResults.pop(0);
        }
    };
    
    var addNewGameToCache = function (gamesResults, timestamp, amount, gameId) {

        gamesResults.push({ Timestamp: timestamp, Amount: amount, GameId: gameId });
    };
    
    var updateCachedGamesLast24Hour = function (gamesResults, timestamp, amount, gameId) {

        removeGamesOlderThan24HoursFromCache(gamesResults, timestamp);
        addNewGameToCache(gamesResults, timestamp, amount, gameId);
    };

    var calculateTotalAmount = function (gamesResults) {

        var total = 0;
        for (var resultIndex = 0; resultIndex < gamesResults.length; resultIndex++) {
            var result = gamesResults[resultIndex];
            total += result.Amount;
        }
        return total;
    };

    var duplicated = function (gamesResults, gameId) {
        
        for (var resultIndex = 0; resultIndex < gamesResults.length; resultIndex++) {

            var result = gamesResults[resultIndex];
            if (result.GameId == gameId) {
                return true;
            }
        }
        
        return false;
    };

    var process = function (state, playerId, timestamp, amount, gameId) {
        
        var gamesResults = state.GamesLast24Hour;

        if (duplicated(gamesResults, gameId)) {
            return state;
        }

        updateCachedGamesLast24Hour(gamesResults, timestamp, amount, gameId);

        if (!isMoreAs24HoursDifference(timestamp, state.LastAlarm)) {
            return state;
        }

        var total = calculateTotalAmount(gamesResults);        
        if (total < -amountLostThreshold) {
            emitAlarm(state, playerId, total, timestamp);
        }
                
        return state;
    };

    var processEvent = function (state, event) {

        var playerId = event.body.PlayerId;
        var timestamp = event.body.Timestamp;
        var amount = event.body.Amount;
        var gameId = event.body.GameId;
        
        return process(state, playerId, timestamp, amount, gameId);
    };

    return {
        init: init,
        processGameLost: processEvent,
        processGameWon: processEvent
    };
};

var detector = irresponsibleGamblingDetector();

fromCategory('Player')
    .foreachStream()
    .when({
        $init: detector.init,
        GameLost: detector.processGameLost,
        GameWon: detector.processGameWon
    });