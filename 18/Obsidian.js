export default class Obsidian{
    cubes;
    rawInput;

    constructor(){
        this.rawInput = [];
    }

    buildCubes = () => {
        this.cubes = new Set(this.rawInput);
    }

    countExposedSides = () => {
        let count = 0;
        this.cubes.forEach(cube => {
            const [x, y, z] =  cube.split(',');
            // above y
            if (!this.cubes.has(`${x},${parseInt(y) + 1},${z}`))
                count++;
            // below y
            if (!this.cubes.has(`${x},${parseInt(y) - 1},${z}`))
                count++;
            // left x
            if (!this.cubes.has(`${parseInt(x) - 1},${y},${z}`))
                count++;
            // right x
            if (!this.cubes.has(`${parseInt(x) + 1},${y},${z}`))
                count++;
            // in front z
            if (!this.cubes.has(`${x},${y},${parseInt(z) - 1}`))
                count++;
            // behind z
            if (!this.cubes.has(`${x},${y},${parseInt(z) + 1}`))
                count++;
        });
        return count;
    }
}