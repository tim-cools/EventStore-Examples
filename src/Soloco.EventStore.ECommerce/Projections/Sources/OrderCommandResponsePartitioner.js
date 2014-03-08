// <reference path="References\1Prelude.js" />
// <reference path="References\Projections.js" />

fromCategory('Order')
    .when({
        CommandResponse: function() {
            linkTo('OrderCommandResponses');
        }
    });