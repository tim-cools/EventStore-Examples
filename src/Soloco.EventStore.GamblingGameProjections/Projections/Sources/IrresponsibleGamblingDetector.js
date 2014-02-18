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
    
    var process = function (previousState, playerId, timestamp, amount, sequenceNumber, e) {
        
        var gamesResults = previousState.GamesLast24Hour;

        for (var resultIndex = 0; resultIndex < gamesResults.length; resultIndex++) {
            var result = gamesResults[resultIndex];
            if (result.SequenceNumber == sequenceNumber) {

                eventServices.emit('IrresponsibleGamblingAlarmsIdempotence', 'CheckFailed', {
                    PlayerId : playerId, SequenceNumber: sequenceNumber
                });

                return previousState;
            }
        }

        while (gamesResults.length > 0 && isMoreAs24HoursDifference(timestamp, gamesResults[0].Timestamp)) {
            gamesResults.pop(0);
        }

        gamesResults.push({ Timestamp: timestamp, Amount: amount, SequenceNumber: sequenceNumber, PlayerId: playerId });
            
        var total = 0;
        for (var resultIndex = 0; resultIndex < gamesResults.length; resultIndex++) {
            var result = gamesResults[resultIndex];
            total += result.Amount;
        }
        
        if (total < -amountLostThreshold && isMoreAs24HoursDifference(timestamp, previousState.LastAlarm)) {
            previousState.LastAlarm = timestamp;
            emitAlarm(playerId, total, timestamp);
        }
        
        return previousState;
    };

    var processEvent = function (previousState, event) {

        var playerId = event.body.PlayerId;
        var timestamp = event.body.Timestamp;
        var amount = event.body.Amount;
        
        return process(previousState, playerId, timestamp, amount, event.sequenceNumber);
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