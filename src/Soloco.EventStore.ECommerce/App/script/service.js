var app = angular.module('app', []);
app.value('$', $);

app.service('orderCreator', ['$', '$rootScope', function ($, $rootScope) {

    var userId = "5654";
    var orderCreatorServer;

    var initialize = function () {

        orderCreatorServer = $.connection.OrderCreator;

        orderCreatorServer.client.feedbackReceived = function (feedback) {
            $rootScope.$emit('feedbackReceived', feedback);
        };

        $.connection.hub.start().pipe(function () {
            initializeData();
        });
    };

    var initializeData = function () {

        orderCreatorServer.server.receiveProducts().done(function (data) {
            $rootScope.$emit('productsReceived', data);
        }).pipe(function(data) {
            orderCreatorServer.server.registerClient(userId);
        });
    };

    var createOrder = function () {
        orderCreatorServer.server.createOrder(userId);
    };

    return {
        initialize: initialize,
        createOrder: createOrder
        //openMarket: openMarket,
        //closeMarket: closeMarket,
        //reset: reset
    };
}
]);