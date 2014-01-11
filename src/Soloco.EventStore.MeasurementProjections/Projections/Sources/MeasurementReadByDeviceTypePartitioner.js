/// <reference path="References\1Prelude.js" />

var deviceTypePartitioner = function ($eventServices) {

    var eventServices = !$eventServices ? { linkTo: linkTo } : $eventServices;

    var storeDeviceType = function (state, eventEnvelope) {
        state.deviceType = eventEnvelope.body.DeviceType;
        return state;
    };

    var linkToDeviceType = function(s, e) {

        if (!s.deviceType) return s;

        eventServices.linkTo("DeviceType-" + s.deviceType, e);

        return s;
    };
    
    return {
        storeDeviceType: storeDeviceType,
        linkToDeviceType: linkToDeviceType
    };
};

var partitioner = deviceTypePartitioner();

fromCategory("Device")
    .foreachStream()
    .when({

        DeviceConfigured: partitioner.storeDeviceType,
        MeasurementRead: partitioner.linkToDeviceType
    });