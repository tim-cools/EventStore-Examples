var orderController = function ($scope, orderCreator) {

    var $rootScope = $scope.$parent;

    $scope.products = [];
    $scope.selectedProduct = {};
    $scope.orderCreating = false;

    $scope.createOrder = function () {
        $scope.orderCreating = true;
        orderCreator.createOrder();
    };
    
    $scope.selectProduct = function (product) {
        $scope.selectedProduct = product;
    };

    function productsReceived(products) {
        $scope.products = products;
    }
    
    function onEvent(eventName, handler) {
        $rootScope.$on(eventName, function(e, status) {
            $scope.$apply(function() {
                handler(status);
            });
        });
    }
    
    onEvent('productsReceived', productsReceived);

    orderCreator.initialize();
}