 <reference path="References\1Prelude.js" />
 <reference path="References\Projections.js" />

var mixin = function mixinConstructor() {

    var add = function (destination, mixin) {
        for (var k in mixin) {
            if (mixin.hasOwnProperty(k)) {
                destination[k] = mixin[k];
            }
        }
        return destination;
    };

    var remove = function (destination, mixin) {
        for (var k in mixin) {
            if (mixin.hasOwnProperty(k)) {
                delete destination[k];
            }
        }
        return destination;
    };

    return {
        add: add,
        remove: remove,
    };
}();

var orderHandler = function orderHandlerConstuctor($eventServices) {

    var eventServices = !$eventServices ? { emit: emit } : $eventServices;

    var orderStatus = {
        open: 'Open',
        closed: 'Closed'
    };

    var commandResultCode = {
        ok: 'OK',
        error: 'Error'
    };

    var getId = function (streamName) {
        var parts = streamName.split('-');
        return parts[parts.length - 1];
    };

    var orderMixin = {

        create: function (command) {

            var orderId = command.orderId;
            var userId = command.userId;

            publish('OrderCreated', {
                orderId: orderId,
                userId: userId
            });
        },

        addLine: function (command) {

            var productId = command.productId;
            var amount = command.amount;

            verifyStatus(orderStatus.open);
            verifyProductNotYetOrdered(productId);
            verifyAmountGreatherAsNull(amount);

            publish('OrderLineRemoved', {
                productId: productId,
                amount: amount
            });
        },

        removeLine: function (command) {

            var productId = command.productId;

            verifyStatus(orderStatus.open);

            publish('OrderLineRemoved', {
                productId: productId
            });
        },

        submit: function () {

            verifyStatus(orderStatus.open);
            verifyAtLeastOneProduct();

            publish('OrderSubmitted', {});
        },

        created: function (event) {

            this.status = orderStatus.open;
            this.orderId = event.OrderId;
            this.userId = event.UserId;
        },

        lineAdded: function (event) {

            this.orderLines.push({
                productId: event.productId,
                amount: event.amount
            });
        },

        lineRemoved: function (event) {

            for (var orderLineIndex = 0; orderLineIndex < this.orderLines.length; orderLineIndex++) {
                var orderLine = orderLineIndex[orderLineIndex];
                if (orderLine.productId == event.productId) {
                    this.orderLines.splice(orderLineIndex, 1);
                    return;
                }
            }

            throw "InvalidProduct";
        },

        submitted: function () {

            this.status = orderStatus.closed;
        },

        verifyStatus: function (status) {

            if (this.status != status) {
                throw "StatusNot" + status;
            }
        },

        verifyAtLeastOneProduct: function () {

            if (this.orderLines.length == 0) {
                throw "NoProductsSelected";
            }
        },

        verifyProductNotYetOrdered: function (productId) {

            for (var orderLineIndex = 0; orderLineIndex < this.orderLines.length; orderLineIndex++) {

                var orderLine = orderLineIndex[orderLineIndex];
                if (orderLine.productId == productId) {
                    throw "ProductAlreadyOrderedOrdered";
                }
            }
        },

        verifyAmountGreatherAsNull: function (amount) {
            if (amount <= 0) {
                throw "AmountGreatherAsNull";
            }
        },

        toState: function () {
            return mixin.remove(this, orderMixin);
        },

        publish: function (name, event) {
            eventServices.emit('Order-' + orderId, name, event);
        }
    };

    var createOrder = function (state) {
        if (state == null) {
            state = { orderLines: [] };
        }

        return mixin.add(state, orderMixin);
    };

    var handleCommand = function (name, state, event) {

        var id = getId(event.streamId);
        var commandId = event.body.commandId;
        var aggregate = createOrder(state);

        try {
            aggregate[name](event.body);
            respond(id, event.eventType, commandId, commandResultCode.ok);
        }
        catch (error) {
            respond(id, event.eventType, commandId, commandResultCode.error, error);
        }

        return state;
    };

    var respond = function (orderId, commandType, commandId, code, error) {
        eventServices.emit('Order-' + orderId, 'CommandResponse', {
            commandId: commandId,
            commandType: commandType,
            code: code,
            error: error
        });
    };

    var handleEvent = function (name, state, event) {

        var aggregate = mixin.add(state, orderMixin);
        aggregate[name](event.body);
        return aggregate.toState();
    };

    return {
        Command: handleCommand,
        Event: handleEvent,
    };
};

var order = orderHandler();

fromCategory('Order')
    .foreachStream()
    .when({
        CreateOrder: function (s, e) { return order.Command('create', s, e); },
        AddOrderLine: function (s, e) { return order.Command('addLine', s, e); },
        RemoveOrderLine: function (s, e) { return order.Command('removeLine', s, e); },
        SubmitOrder: function (s, e) { return order.Command('submit', s, e); },

        OrderCreated: function (s, e) { return order.Event('created', s, e); },
        OrderLineAdded: function (s, e) { return order.Event('lineAdded', s, e); },
        OrderLineRemoved: function (s, e) { return order.Event('lineRemoved', s, e); },
        OrderSubmitted: function (s, e) { return order.Event('submitted', s, e); },
    });