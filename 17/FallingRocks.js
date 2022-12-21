const ROCK = '#';
const AIR = '.';

class Rocknomino{
    points;
    x; // x and y are of bottom left most point
    y;
    width;
    type

    constructor(type, startingX, startingY){
        this.type = type;
        this.x = startingX;
        this.y = startingY;
        this.points = this.determinePoints(type);
    }

    getHighestX = () => {
        return this.points.reduce((a, b) => a.x > b.x ? a : b, 0).x + 1;
    }

    determinePoints = (type) => {
        // Assumes bottom left of grid is 0,0
        let points = [];
        switch (type) {
            case 'h-line':
                points.push({x: this.x, y: this.y});
                points.push({x: this.x, y: this.y + 1});
                points.push({x: this.x, y: this.y + 2});
                points.push({x: this.x, y: this.y + 3});
                this.width = 4;
                break;
            case 'v-line':
                points.push({x: this.x, y: this.y});
                points.push({x: this.x + 1, y: this.y});
                points.push({x: this.x + 2, y: this.y});
                points.push({x: this.x + 3, y: this.y});
                this.width = 1;
                break;
            case 'plus':
                points.push({x: this.x, y: this.y + 1});
                points.push({x: this.x + 1, y: this.y});
                points.push({x: this.x + 1, y: this.y + 1});
                points.push({x: this.x + 1, y: this.y + 2});
                points.push({x: this.x + 2, y: this.y + 1});
                this.width = 3;
                break;
            case 'L':
                points.push({x: this.x, y: this.y});
                points.push({x: this.x, y: this.y + 1});
                points.push({x: this.x, y: this.y + 2});
                points.push({x: this.x + 1, y: this.y + 2});
                points.push({x: this.x + 2, y: this.y + 2});
                this.width = 3;
                break;
            case 'square':
                points.push({x: this.x, y: this.y});
                points.push({x: this.x + 1, y: this.y});
                points.push({x: this.x, y: this.y + 1});
                points.push({x: this.x + 1, y: this.y + 1});
                this.width = 2;
                break;
        }
        return points;
    }
}

export default class FallingRocks{
    jetPattern;
    jetPointer;
    grid;
    maxWidth;
    currentHeight;
    rocks;
    rockPointer;
    highestX;

    constructor(maxWidth, startingHeight){
        this.jetPattern = [];
        this.jetPointer = 0;
        this.maxWidth = maxWidth;
        this.currentHeight = startingHeight;
        this.grid = [];
        this.buildGrid(maxWidth, startingHeight);
        this.rocks = [];
        this.queueRocks();
        this.rockPointer = 0;
        this.highestX = 0;
    }

    parsePattern = () => {
        this.jetPattern = this.jetPattern.split('');
    }

    buildGrid = (width, height) => {
        for (let x = 0; x < height; x++){
            this.grid.push([]);
            for (let y = 0; y < width; y++){
                this.grid[x].push(AIR);
            }
        }
    }

    queueRocks = () => {
        this.rocks.push('h-line');
        this.rocks.push('plus');
        this.rocks.push('L');
        this.rocks.push('v-line');
        this.rocks.push('square');
    }

    printGrid = () => {
        let printString = '';
        for (let x = this.grid.length - 1; x >= 0; x--){
            for (let y = 0; y < this.grid[x].length; y++){
                printString += this.grid[x][y];
            }
            printString += '\n';
        }
        printString += '\n';
        console.log(printString);
    }

    getHighestRockRow = () => {
        let hasRock = false;
        let x = -1;
        do{
            x++;
            hasRock = false;
            innerLoop: for (let y = 0; y < this.grid[x].length; y++){
                if (this.grid[x][y] == ROCK){
                    hasRock = true;
                    break innerLoop;
                }
            }
        } while (hasRock);
        return x;
    }

    dropManyRocks = (numberOfRocks) => {
        for (let i = 0; i < numberOfRocks; i++){
            this.dropRock();
            //this.printGrid();
        }
    }

    dropRock = () => {
        const startingPoint = {x: this.highestX + 3, y: 2};
        let currentRockType = this.rocks[this.rockPointer];
        this.rockPointer = (this.rockPointer + 1) % this.rocks.length;
        let currentRock = new Rocknomino(currentRockType, startingPoint.x, startingPoint.y);

        rockfall: for (let tick = 0; tick < this.jetPattern.length; tick++){
            // Blow left or right?
            if (this.jetPattern[this.jetPointer] == '<'){
                // Is left viable?
                if (this.checkLeft(currentRock)){
                    // Then adjust points
                    currentRock.points.forEach(point => point.y--);
                }
                
            } else {
                // Is right viable?
                if (this.checkRight(currentRock)){
                    // Then adjust points
                    currentRock.points.forEach(point => point.y++);
                }
            }
            this.jetPointer = (this.jetPointer + 1) % this.jetPattern.length;

            // Can rock fall down one?
            if(this.checkDown(currentRock)){
                // If yes, adjust points
                currentRock.points.forEach(point => point.x--);
            } else {
                // Otherwise set points on grid to rocks
                currentRock.points.forEach(point => this.grid[point.x][point.y] = ROCK);
                if (currentRock.getHighestX() > this.highestX)
                    this.highestX = currentRock.getHighestX();
                break rockfall;
            }
        }

        // Rock has been placed, do we need to expand the grid x scale?
        if (this.grid.length - this.highestX < 10){
            this.expandGrid();
        }

    }

    expandGrid = () => {
        for (let x = 0; x < 10; x++){
            this.grid.push([]);
            for (let y = 0; y < this.maxWidth; y++){
                this.grid[this.grid.length - 1].push(AIR);
            }
        }
    }

    checkLeft = (currentRock) => {
        return currentRock.points.every(point => point.y - 1 >= 0 && this.grid[point.x][point.y - 1] != ROCK);
    }

    checkRight = (currentRock) => {
        return currentRock.points.every(point => point.y + 1 < this.maxWidth && this.grid[point.x][point.y + 1] != ROCK);
    }

    checkDown = (currentRock) => {
        return currentRock.points.every(point => point.x - 1 >= 0 && this.grid[point.x - 1][point.y] != ROCK);
    }
}