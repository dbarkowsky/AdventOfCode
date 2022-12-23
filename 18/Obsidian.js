export default class Obsidian{
    rockCubes; // A set of rock coordinates
    waterCubes; // A set of water coordinates
    rawInput; // Raw string input from scan.txt. Represents rock locations

    constructor(){
        this.rawInput = [];
    }

    // Puts rocks into the set
    placeRocks = () => {
        this.rockCubes = new Set(this.rawInput);
    }

    // Counts the exposed sides of the obsidian cubes
    // If !withWater, any side exposed to air counts
    // If withWater, only sides exposed to water (not interior) count
    countExposedSides = (withWater = false) => {
        let count = 0;
        this.rockCubes.forEach(cube => {
            const [x, y, z] =  cube.split(',').map(el => parseInt(el));
            // above y
            const aboveY = `${x},${y + 1},${z}`;
            if (
                (!withWater && !this.rockCubes.has(aboveY)) ||
                (withWater && this.waterCubes.has(aboveY))
            )
                count++;
            // below y
            const belowY = `${x},${y - 1},${z}`;
            if (
                (!withWater && !this.rockCubes.has(belowY)) ||
                (withWater && this.waterCubes.has(belowY))
            )
                count++;
            // left x
            const leftX = `${x - 1},${y},${z}`;
            if (
                (!withWater && !this.rockCubes.has(leftX)) ||
                (withWater && this.waterCubes.has(leftX))
            )
                count++;
            // right x
            const rightX = `${x + 1},${y},${z}`;
            if (
                (!withWater && !this.rockCubes.has(rightX)) ||
                (withWater && this.waterCubes.has(rightX))
            )
                count++;
            // in front z
            const frontZ = `${x},${y},${z - 1}`;
            if (
                (!withWater && !this.rockCubes.has(frontZ)) ||
                (withWater && this.waterCubes.has(frontZ))
            )
                count++;
            // behind z
            const backZ = `${x},${y},${z + 1}`;
            if (
                (!withWater && !this.rockCubes.has(backZ)) ||
                (withWater && this.waterCubes.has(backZ))
            )
                count++;
        });
        return count;
    }

    // Fills an area around the obsidian rock with 'water' using a flood-fill pattern
    // Water locations added to waterCubes
    fillWithWater = () => {
        this.waterCubes = new Set();
        const limits = this.findLimits();

        const startingNode = `${limits.xMin},${limits.yMin},${limits.zMin}`;
        const floodQueue = [];
        floodQueue.push(startingNode);

        while (floodQueue.length > 0){
            const currentCube = floodQueue.shift();
            if (
                !this.waterCubes.has(currentCube) &&
                !this.rockCubes.has(currentCube) &&
                this.isWithinLimits(currentCube, limits)
            ){
                this.waterCubes.add(currentCube);
                const [x, y, z] =  currentCube.split(',').map(el => parseInt(el));
                // above y
                floodQueue.push(`${x},${y + 1},${z}`);
                // below y
                floodQueue.push(`${x},${y - 1},${z}`);
                // left x
                floodQueue.push(`${x - 1},${y},${z}`);
                // right x
                floodQueue.push(`${x + 1},${y},${z}`);
                // in front z
                floodQueue.push(`${x},${y},${z - 1}`);
                // behind z
                floodQueue.push(`${x},${y},${z + 1}`);
            }
        }
    }

    // Checks if a cube is within the limits of the experiment
    isWithinLimits = (cube, limits) => {
        const [x, y, z] =  cube.split(',').map(el => parseInt(el));
        // Adding -1/+1 to allow for water to flow outside of max and min blocks
        if (
            x >= limits.xMin - 1 &&
            x <= limits.xMax + 1 &&
            y >= limits.yMin - 1 &&
            y <= limits.yMax + 1 &&
            z >= limits.zMin - 1 &&
            z <= limits.zMax + 1
        ) return true;
        return false;
    }

    // Finds the exterior dimensions of the obsidian
    findLimits = () => {
        let xMin, yMin, zMin;
        xMin = yMin = zMin = Infinity;
        let xMax, yMax, zMax;
        xMax = yMax = zMax = 0;

        this.rockCubes.forEach(cube => {
            const [x, y, z] =  cube.split(',').map(el => parseInt(el));
            xMin = x < xMin ? x : xMin;
            xMax = x > xMax ? x : xMax;
            yMin = y < yMin ? y : yMin;
            yMax = y > yMax ? y : yMax;
            zMin = z < zMin ? z : zMin;
            zMax = z > zMax ? z : zMax;
        })

        return { xMin, xMax, yMin, yMax, zMin, zMax }
    }
}