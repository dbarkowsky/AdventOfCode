export default class SensorSuite{
    sensors;
    beacons;
    readings;

    constructor(){
        this.sensors = [];
        this.beacons = [];
        this.readings = [];
    }

    countCoverageOnRow = (rowNumber) => {
        console.log('Starting count');
        const coverageSet = new Set();
        this.sensors.forEach(sensor => {
            sensor.coveredPoints.forEach(point => {
                if (
                    point.x == rowNumber &&
                    this.isNotABeacon(point.x, point.y) &&
                    this.isNotASensor(point.x, point.y)
                ){
                    coverageSet.add(JSON.stringify(point));
                }
            })
        });
        return coverageSet.size;
    }

    isNotABeacon = (xCoord, yCoord) => {
        return this.beacons.every(beacon => !(beacon.x == xCoord && beacon.y == yCoord)); // NAND condition
    }

    isNotASensor = (xCoord, yCoord) => {
        return this.sensors.every(sensor => !(sensor.x == xCoord && sensor.y == yCoord)); // NAND condition
    }

    parseReadings = () => {
        console.log('Parsing Started');
        this.readings.forEach(reading => {
            // I swap X and Y here. It just makes more sense when viewing the hypothetical grid
            const sensorY = parseInt(reading.substring(reading.indexOf('x=') + 2, reading.indexOf(',')));
            const sensorX = parseInt(reading.substring(reading.indexOf('y=') + 2, reading.indexOf(':')));
            const beaconY = parseInt(reading.substring(reading.indexOf('x=', reading.indexOf('closest')) + 2, reading.indexOf(',', reading.indexOf('closest'))));
            const beaconX = parseInt(reading.substring(reading.indexOf('y=', reading.indexOf('closest')) + 2));

            this.beacons.push({x: beaconX, y: beaconY});
            this.sensors.push(this.createSensor(sensorX, sensorY, beaconX, beaconY));
        })
        console.log('Parsing Finished');
    }

    createSensor = (xCoord, yCoord, beaconX, beaconY) => {
        console.log(`Creating sensor {${xCoord}, ${yCoord}}, beacon {${beaconX}, ${beaconY}}`);
        let sensorObj = {
            x: xCoord,
            y: yCoord,
            coveredPoints: []
        }

        const sensorRange = Math.abs(Math.abs(beaconX) - Math.abs(xCoord)) + Math.abs(Math.abs(beaconY) - Math.abs(yCoord));
        let rightY = sensorObj.y;
        let leftY = sensorObj.y;

        // Top of diamond to centre
        for (let i = sensorObj.x - sensorRange; i < sensorObj.x; i++){
            //console.log(sensorObj.x - sensorRange, sensorObj.x,i);
            let currentY = leftY;
            while (currentY <= rightY){
                //console.log(currentY, rightY);
                sensorObj.coveredPoints.push({
                    x: i,
                    y: currentY
                });
                currentY++;
            }
            rightY++;
            leftY--;
        }

        // Centre (inclusive) to bottom of diamond
        for (let i = sensorObj.x; i <= sensorObj.x + sensorRange; i++){
            let currentY = leftY;
            while (currentY <= rightY){
                sensorObj.coveredPoints.push({
                    x: i,
                    y: currentY
                });
                currentY++;
            }
            rightY--;
            leftY++;
        }
        console.log(`Sensor created {${xCoord}, ${yCoord}}`);
        return sensorObj;
    }
}