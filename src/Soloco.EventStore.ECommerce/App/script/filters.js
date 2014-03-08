app.filter('percentage', function () {
    return function (changeFraction) {
        return (changeFraction * 100).toFixed(2) + "%";
    }
});

app.filter('change', function () {
    return function (changeAmount) {
        if (changeAmount > 0) {
            return "▲ " + changeAmount.toFixed(2);
        }
        else if (changeAmount < 0) {
            return "▼ " + changeAmount.toFixed(2);
        }
        else {
            return changeAmount.toFixed(2);
        }
    }
});