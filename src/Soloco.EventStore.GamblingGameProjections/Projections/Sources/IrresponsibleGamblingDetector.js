// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

var irresponsibleGamblingDetector = function irresponsibleGamblingDetectorConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var millisecondsPerDay = 24 * 60 * 60 * 1000;
    var amountLostThreshold = 500;
    
    var createEvent = function (playerId, totalAmount, timestamp) {
        return {
            PlayerId: playerId,
            AmountSpendLAst24Hours: totalAmount,
            Timestamp: timestamp
        };
    };

    var emitAlarm = function (playerId, total, timestamp)  {

        var event = createEvent(playerId, total, timestamp);

        eventServices.emit('IrresponsibleGamblingAlarms', 'IrresponsibleGamblerDetected', event);
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
    
    var addNewGameToCache = function (gamesResults, timestamp, amount, sequenceNumber) {

        gamesResults.push({ Timestamp: timestamp, Amount: amount, SequenceNumber: sequenceNumber });
    };
    
    var updateChachedGamesLast24Hour = function (gamesResults, timestamp, amount, sequenceNumber) {

        removeGamesOlderThan24HoursFromCache(gamesResults, timestamp);
        addNewGameToCache(gamesResults, timestamp, amount, sequenceNumber);
    };

    var calculateTotalAmount = function (gamesResults) {

        var total = 0;
        for (var resultIndex = 0; resultIndex < gamesResults.length; resultIndex++) {
            var result = gamesResults[resultIndex];
            total += result.Amount;
        }
        return total;
    };

    var idempotent = function (gamesResults, sequenceNumber) {
        
        for (var resultIndex = 0; resultIndex < gamesResults.length; resultIndex++) {

            var result = gamesResults[resultIndex];
            if (result.SequenceNumber == sequenceNumber) {
                return false;
            }
        }
        
        return true;
    };

    var process = function (state, playerId, timestamp, amount, sequenceNumber) {
        
        var gamesResults = state.GamesLast24Hour;

        if (!idempotent(gamesResults, sequenceNumber)) {
            return state;
        }

        updateChachedGamesLast24Hour(gamesResults, timestamp, amount, sequenceNumber);

        var total = calculateTotalAmount(gamesResults);
        
        if (total < -amountLostThreshold && isMoreAs24HoursDifference(timestamp, state.LastAlarm)) {
            
            state.LastAlarm = timestamp;
            emitAlarm(playerId, total, timestamp);
        }
        
        return state;
    };

    var processEvent = function (state, event) {

        var playerId = event.body.PlayerId;
        var timestamp = event.body.Timestamp;
        var amount = event.body.Amount;
        
        return process(state, playerId, timestamp, amount, event.sequenceNumber);
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