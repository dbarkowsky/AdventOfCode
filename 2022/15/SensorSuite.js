export default class SensorSuite{
    sensors;
    readings;

    constructor(){
        this.sensors = [];
        this.readings = [];
    }
    
    // *****************************************************************
    // Revised attempt. Approach used by https://observablehq.com/@mbostock

    // Parses input and adds sensor node to list.
    parseReadings = () => {
        this.readings.forEach(reading => {
            // I swap X and Y here. It just makes more sense when viewing the hypothetical grid
            const sensorY = parseInt(reading.substring(reading.indexOf('x=') + 2, reading.indexOf(',')));
            const sensorX = parseInt(reading.substring(reading.indexOf('y=') + 2, reading.indexOf(':')));
            const beaconY = parseInt(reading.substring(reading.indexOf('x=', reading.indexOf('closest')) + 2, reading.indexOf(',', reading.indexOf('closest'))));
            const beaconX = parseInt(reading.substring(reading.indexOf('y=', reading.indexOf('closest')) + 2));

            this.sensors.push({
                x: sensorX, 
                y: sensorY, 
                bx: beaconX, 
                by: beaconY,
                distance: this.getDistance(sensorX, sensorY, beaconX, beaconY)
            });
        })
    }

    // Gets distance between two points
    getDistance = (x1, y1, x2, y2) => Math.abs(x2 - x1) + Math.abs(y2 - y1);

    // Counts the number of spaces in a row that are included in any sensor's coverage
    countCoverageOnRow = (rowNumber) => {
        // Get largest and smallest sensor Y position
        // Get largest distance of any sensor
        let bigY = 0;
        let smallY = Infinity;
        let bigDistance = 0;
        
        this.sensors.forEach(sensor => {
            bigY = bigY < sensor.y ? sensor.y : bigY;
            smallY = smallY > sensor.y ? sensor.y : smallY;
            bigDistance = bigDistance < sensor.distance ? sensor.distance : bigDistance;
        })

        // From smallest Y to biggest Y plus the biggest distance (range), count coverage blocks
        let count = 0;
        range: for (let y = smallY - bigDistance; y <= bigY + bigDistance; y++){
            // Check all sensors
            // If distance to location is <= distance to beacon, it is covered
            // But also only if beacon is not on this location!
            sensors: this.sensors.every(sensor => {
                if (
                    this.isCovered(sensor, rowNumber, y) &&
                    !(sensor.bx == rowNumber && sensor.by == y)
                ){
                    count++;
                    return false;
                }
                return true;
            })
        }
        return count;
    }

    // Checks if a space is covered by a specific sensor
    isCovered = (sensor, x, y) => (this.getDistance(sensor.x, sensor.y, sensor.bx, sensor.by) >= this.getDistance(sensor.x, sensor.y, x, y));
    
    // Checks if a space is covered by any sensor
    checkCoverageAllSensors = (x, y) => {
        let found = false;
        this.sensors.forEach(sensor => {
            if (!found && this.isCovered(sensor, x, y))
                found = true;
        });
        return found;
    }

    // Finds the one place a beacon must be (the place with no coverage, within the MAX range)
    findBeacon = (MAX) => {
        let location = {x: 0, y: 0};
        // For each sensor
        this.sensors.every(sensor => {
            // Look at all points just outside the sensor's range
            // Check each of these points for coverage
            // If not covered, return location
            let rightY = sensor.y;
            let leftY = sensor.y;

            // Top of diamond to centre
            for (let x = sensor.x - sensor.distance; x < sensor.x; x++){
                // First iteration, check above
                if (x == sensor.x){
                    if (this.isWithinBounds(x - 1, sensor.y, MAX) && !this.checkCoverageAllSensors(x - 1, sensor.y)){
                        location = {x: x - 1, y: sensor.y};
                        return false;
                    }  
                }

                // All iterations
                // Look to left
                if (this.isWithinBounds(x, leftY - 1, MAX) && !this.checkCoverageAllSensors(x, leftY - 1)){
                    location = {x: x, y: leftY - 1};
                    return false;
                }
                    
                // Look to right
                if (this.isWithinBounds(x, rightY + 1, MAX) && !this.checkCoverageAllSensors(x, rightY + 1)){
                    location = {x: x, y: rightY + 1};
                    return false;
                }
                    
                rightY++;
                leftY--;
            }

            // Centre (inclusive) to bottom of diamond
            for (let x = sensor.x; x <= sensor.x + sensor.distance; x++){
                // Last iteration only
                if (x == sensor.x + sensor.distance){
                    if (this.isWithinBounds(x + 1, sensor.y, MAX) && !this.checkCoverageAllSensors(x + 1, sensor.y)){
                        location = {x: x + 1, y: sensor.y};
                        return false;
                    }
                }

                // All iterations
                // Look to left
                if (this.isWithinBounds(x, leftY - 1, MAX) && !this.checkCoverageAllSensors(x, leftY - 1)){
                    location = {x: x, y: leftY - 1};
                    return false;
                }
                    
                // Look to right
                if (this.isWithinBounds(x, rightY + 1, MAX) && !this.checkCoverageAllSensors(x, rightY + 1)){
                    location = {x: x, y: rightY + 1};
                    return false;
                }   

                rightY--;
                leftY++;
            }
            return true;
        })
        console.log(location);
        return location;
    }

    // Checks if point is within max range
    isWithinBounds = (x, y, MAX) => {
        if (y >= 0 && y <= MAX && x >= 0 && x <= MAX)
            return true;
        return false;
    }

    // Calculates frequency. Frequency is always the same, but MAX range may change.
    calculateFrequency = (MAX) => {
        const multiplier = 4000000;
        const {x, y} = this.findBeacon(MAX);
        return (multiplier * y) + x;
    }


    // **********************************************************************************************
    // Original attempt below. Works for small sets, but caused heap overflow with very large numbers.
    countCoverageOnRowBrute = (rowNumber) => {
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

    parseReadingsBrute = () => {
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