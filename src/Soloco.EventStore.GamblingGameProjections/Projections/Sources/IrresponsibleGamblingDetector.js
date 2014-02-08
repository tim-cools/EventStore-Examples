// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

var irresponsibleGamblingDetector = function irresponsibleGamblingDetectorConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var createEvent = function (playerId, totalAmount) {
        return {
            PlayerId: playerId,
            TotalAmount: totalAmount,
        };
    };

    var emitAlarm = function (playerId, total)  {

        var event = createEvent(playerId, total);

        eventServices.emit('IrresponsibleGamblingAlarms', 'IrresponsibleGamblerDetected', event);
    };

    var init = function () {
        return {            
            LastAlarm: null,
            GamesLast24Hour: []
        };
    };

    var isMoreAs24HoursDifference = function (timestamp, reference) {

        if (timestamp === null) return true;
        
    };
    
    var process = function (previousState, playerId, timestamp, amount) {

        var gamesResults = previousState.GamesLast24Hour;

        while (gamesResults.count > 0 && isMoreAs24HoursDifference(gamesResults[0].Timestamp, timestamp)) {
            previousState.pop(0);
        }

        gamesResults.push({ Timestamp: timestamp, Amount: amount });
            
        var total = 0;
        for (var resultIndex = 0; resultIndex < gamesResults; resultIndex++) {
            var result = gamesResults;
            total += result.Amount;
        }
        
        if (total > 500 && isMoreAs24HoursDifference(previousState.LastAlarm, timestamp)) {
            previousState.LastAlarm = timestamp;
            emitAlarm(playerId, total);
        }
        
        return previousState;
    };

    var processGameLost = function (previousState, measurementEvent) {

        var playerId = measurementEvent.body.PlayerId;
        var timestamp = measurementEvent.body.Timestamp;
        var amount = measurementEvent.body.Amount;

        return process(previousState, playerId, timestamp, amount);
    };

    var processGameWon = function (previousState, measurementEvent) {

        var timestamp = measurementEvent.body.Timestamp;
        var amount = measurementEvent.body.Amount;

        return process(previousState, timestamp, amount);
    };

    return {
        init: init,
        processGameLost: processGameLost,
        processGameWon: processGameWon
    };
};

var detector = irresponsibleGamblingDetector();

fromCategory('Game')
    .foreachStream()
    .when({
        GameLost: detector.processGameLost,
        GameWon: detector.processGameWon
    });