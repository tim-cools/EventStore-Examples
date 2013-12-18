/// <reference path="References\1Prelude.js" />

fromAll().when({
    'MeasurementRead': function (s, e) {

        if (e.body == null) return s;

        linkTo('MeasurementRead', e);

        return s;
    }
});