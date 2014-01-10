/// <reference path="References\1Prelude.js" />

fromCategory("Meter")
    .foreachStream()
    .when({

        DeviceConfigured: function (s, e) {

            return { DeviceType: e.body.DeviceType };
        },
    
        MeasurementRead: function (s, e) {

            if (!s.DeviceType) return s;
            
            linkTo("DeviceType-" + s.DeviceType, e);

            return s;
        }
    });