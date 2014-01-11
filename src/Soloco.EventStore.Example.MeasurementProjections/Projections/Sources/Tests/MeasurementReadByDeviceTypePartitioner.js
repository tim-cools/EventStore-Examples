/// <reference path="../References/jasmine/jasmine.js"/>
/// <reference path="../MeasurementReadByDeviceTypePartitioner.js"/>

describe("when partioning Device MeasurementRead events by type", function () {
    
    var partitioner;
    var projections;

    beforeEach(function () {
        projections = jasmine.createSpyObj('projections', ['linkTo']);
        partitioner = deviceTypePartitioner(projections);
    });

    describe("when storing devicetype", function () {

        it("should store the deviceType in the state", function () {
            var state = partitioner.storeDeviceType({}, { body: { DeviceType: 'someDeviceType' } });
            expect(state.deviceType).toEqual('someDeviceType');
        });
    });

    describe("when linking devicetype", function () {

        describe("given devicetype is not", function () {

            var state;

            beforeEach(function () {
                state = partitioner.linkToDeviceType({ }, { body: { } });
            });
            
            it("should return the previous state", function () {
                expect(state).toEqual({ });
            });
            
            it("should not link the event", function () {
                expect(projections.linkTo).wasNotCalled();
            });
        });

        describe("given devicetype is set", function () {

            var state;

            beforeEach(function () {
                state = partitioner.linkToDeviceType(
                    { deviceType: 'someDeviceType' },
                    { body: { someEvent: 'someDate' } });
            });

            it("should return the previous state", function () {
                expect(state).toEqual({ deviceType: 'someDeviceType' });
            });

            it("should link the event", function () {
                expect(projections.linkTo)
                    .wasCalledWith("DeviceType-someDeviceType", { body: { someEvent: 'someDate' } });
            });
        });
    });
});