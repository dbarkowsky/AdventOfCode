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
    jetPattern; // List of instructions for jet stream movements
    jetPointer; // Where in the instructions we currently are
    grid; // 2D list that holds fallen rocks.
    maxWidth; // Maximum width of grid
    currentHeight; // Current tracked height of grid
    rocks;  // List of Rocknomino types
    rockPointer; // Where in the rocks list we currently are
    highestX; // Highest x value in grid that contains rocks
    snapshots; // A list of snapshots that capture last n rows, rocks dropped, and height at points in time. Used for pattern checking.

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
        this.snapshots = [];
    }

    // Breaks a pattern string into list
    parsePattern = () => {
        this.jetPattern = this.jetPattern.split('');
    }

    // Builds a grid that only contains AIR
    buildGrid = (width, height) => {
        for (let x = 0; x < height; x++){
            this.grid.push([]);
            for (let y = 0; y < width; y++){
                this.grid[x].push(AIR);
            }
        }
    }

    // Builds the list of rocks
    queueRocks = () => {
        this.rocks.push('h-line');
        this.rocks.push('plus');
        this.rocks.push('L');
        this.rocks.push('v-line');
        this.rocks.push('square');
    }

    // Prints the grid along with row numbers
    printGrid = () => {
        let printString = '';
        for (let x = this.grid.length - 1; x >= 0; x--){
            printString += `${x}\t`;
            for (let y = 0; y < this.grid[x].length; y++){
                printString += this.grid[x][y];
            }
            printString += '\n';
        }
        printString += '\n';
        console.log(printString);
    }

    // Determines the highest x value that contains a rock
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

    // Builds a pattern string from the top x rows. Used to check for patterns
    getPatternString = (patternBlockSize) => {
        let patternString = '';
        for (let x = this.highestX - 1; x > this.highestX - patternBlockSize; x--){
            for (let y = 0; y < this.grid[x].length; y++){
                patternString += this.grid[x][y];
            }
        }
        return patternString;
    }

    // Drops many rocks. If only a few, does normally, otherwise uses pattern checking.
    // args = number of rocks to drop, how many rocks dropped before checking for pattern, size of pattern blocks in rows
    dropManyRocks = (numberOfRocks, rocksBeforePattern = 1000, patternBlockSize = 100) => {
        // Pattern can't be bigger or equal to number of rocks otherwise it doesn't make sense.
        // Ideally this would be handled better....
        if (patternBlockSize >= numberOfRocks) patternBlockSize = Math.floor(numberOfRocks / 10);

        // If it's a small amount that doesn't require a pattern, just drop the rocks
        if (numberOfRocks < rocksBeforePattern){
            for (let i = 0; i < numberOfRocks; i++){
                this.dropRock();
            }
        } else {
            // Here we need to do four things:
            // 1. Drop initial rocks
            for (let i = 0; i < rocksBeforePattern; i++){
                this.dropRock();
                if (i >= patternBlockSize - 1){
                    this.snapshots.push({
                        height: this.highestX,
                        rockNum: i + 1,
                        patternString: this.getPatternString(patternBlockSize)
                    })
                }
            }

            // 2 .Check for pattern. If pattern not found, drop another rock and check again
            let patternFound = false;
            let lastPatternBlock;
            let matchingPatternBlock
            do {
                // Get last pattern
                lastPatternBlock = this.snapshots[this.snapshots.length - 1];
                // Try to find a snapshot that has a matching patternString. Slice off last element.
                matchingPatternBlock = [...this.snapshots].slice(0, -1).find(snapshot => snapshot.patternString == lastPatternBlock.patternString);
                if (matchingPatternBlock){
                    patternFound = true;
                } else {
                    this.dropRock();
                    this.snapshots.push({
                        height: this.highestX,
                        rockNum: lastPatternBlock.rockNum + 1,
                        patternString: this.getPatternString(patternBlockSize)
                    })
                }
            } while (!patternFound);

            const [rocksDroppedBeforePattern, heightBeforePattern] = [matchingPatternBlock.rockNum, matchingPatternBlock.height];
            const [rocksDroppedEachPattern, heightGainedEachPattern] = [lastPatternBlock.rockNum - rocksDroppedBeforePattern, lastPatternBlock.height - heightBeforePattern];

            // 3. Multiply pattern to get close to rocks dropped goal
            let remainingRocksToDrop = numberOfRocks - rocksDroppedBeforePattern - rocksDroppedEachPattern;
            const numberOfPossiblePatternsLeft = Math.floor(remainingRocksToDrop / rocksDroppedEachPattern);
            const heightGainedWhenRepeatingPattern = heightGainedEachPattern * numberOfPossiblePatternsLeft;

            // 4. Drop remaining rocks to top up count
            remainingRocksToDrop = remainingRocksToDrop % rocksDroppedEachPattern;
            for (let i = 0; i < remainingRocksToDrop; i++){
                this.dropRock();
            }
            
            this.highestX += heightGainedWhenRepeatingPattern;
        }
    }

    // Drops a single rock onto the grid. 
    dropRock = () => {
        // Determine rock type and starting point
        const startingPoint = {x: this.highestX + 3, y: 2};
        let currentRockType = this.rocks[this.rockPointer];
        this.rockPointer = (this.rockPointer + 1) % this.rocks.length;
        let currentRock = new Rocknomino(currentRockType, startingPoint.x, startingPoint.y);

        // Fall until rock stops
        rockfall: while(true){
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

    // Adds extra rows to the grid. Only need to add a few at a time.
    expandGrid = () => {
        for (let x = 0; x < 10; x++){
            this.grid.push([]);
            for (let y = 0; y < this.maxWidth; y++){
                this.grid[this.grid.length - 1].push(AIR);
            }
        }
    }

    // Checks if Rocknomino can move left
    checkLeft = (currentRock) => {
        return currentRock.points.every(point => point.y - 1 >= 0 && this.grid[point.x][point.y - 1] != ROCK);
    }

    // Checks if Rocknomino can move right
    checkRight = (currentRock) => {
        return currentRock.points.every(point => point.y + 1 < this.maxWidth && this.grid[point.x][point.y + 1] != ROCK);
    }

    // Checks if Rocknomino can move down
    checkDown = (currentRock) => {
        return currentRock.points.every(point => point.x - 1 >= 0 && this.grid[point.x - 1][point.y] != ROCK);
    }
}