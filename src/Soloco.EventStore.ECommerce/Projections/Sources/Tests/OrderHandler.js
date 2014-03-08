// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

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

    var id = function() {

    };
	
    var orderMixin = {
	
        createOrder: function (command) {
		
            var orderId = command.orderId;
			
            publish('OrderCreated', { 
                orderId: orderId
            });
        },
		
        addOrderLine: function (command) {			
		
            var productId = command.productId;			
            var amount = command.amount;
		
            verifyStatus(orderStatus.open);			
            verifyProductNotYetOrdered(productId);			
            verifyAmountGreatherAsNull(amountId);
			
            publish('OrderCreated', { 
                productId: productId, 
                amount: amount
            });
        },
		
        submitOrder: function () {
            
            verifyStatus(orderStatus.open);
            verifyAtLeastOneProduct();
			
            publish('OrderSubmitted', { });
        },
		
        orderCreated: function (event) {
			
            this.status = orderStatus.open;
            this.orderId = event.OrderId;
        },
		
        orderLineAdded: function (event) {
            
            this.orderLines.push({
                productId: event.productId,
                amount: event.amount
            });
        },
		
        orderSubmited: function () {
            
            this.status = orderStatus.Closed;
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

        verifyProductNotYetOrdered: function(productId) {

            for (var orderLineIndex = 0; orderLineIndex < this.orderLines.length; orderLineIndex++) {

                var orderLine = orderLineIndex[orderLineIndex];
                if (orderLine.productId == productId) {
                    throw "ProductAlreadyOrderedOrdered";
                }
            }
        },

        verifyAmountGreatherAsNull: function(amount) {
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

    var createOrder = function(state) {
        if (state == null) {
            state = { orderLines: [] };
        }

        return mixin.add(state, orderMixin);
    };

    var handleCommand = function (name, state, event) {
		
        var id = id(event.streamId);
        var commandId = event.body.commandId;
        var aggregate = createOrder(state);
		
        try {
            aggregate[name](event.body);
            respond(id, commandId, commandResultCode.ok);
        }
        catch (error){
            respond(id, commandId, commandResultCode.error, error);
        }
		
        return state;
    };
	
    var respond = function (orderId, commandId, code, error) {
        eventServices.emit('Order-' + orderId, 'CommandResponse', {
            commandId: commandId,
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

var order = orderAggregate();

fromCategory('Order')
    .foreachStream()
    .when({
        CreateOrder: function(s,e) { return order.Command('CreateOrder', s, e); },
        AddOrderLine: function(s,e) { return order.Command('AddOrderLine', s, e); },
        RemoveOrderLine: function(s,e) { return order.Command('RemoveOrderLine', s, e); },
        SubmitOrder: function(s,e) { return order.Command('SubmitOrder', s, e); },       
		
        OrderCreated: function(s,e) { return order.Event('OrderCreated', s, e); },
        OrderLineAdded: function(s,e) { return order.Event('OrderLineAdded', s, e); },
        OrderLineRemoved: function(s,e) { return order.Event('OrderLineRemoved', s, e); },
        OrderSubmit: function(s,e) { return order.Event('OrderSubmit', s, e); },
    });