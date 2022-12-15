const AIR = 0;
const ROCK = 1;
const SAND = 2;

class Node {
    contents; // node's contents
    x; // x coordinate
    y; // y coordinate

    constructor(x, y, contents = AIR){
        this.contents = contents;
        this.x = x;
        this.y = y;
    }
}

export default class SandTrap{
    grid; // 2D list to manage Nodes
    rockInstructions; // List of instructions to draw rock
    lowestY; // to track smallest Y value
    highestY; // to track largest Y value
    highestX; // to track largest X value
    sandStart; // where sand enters from. object with x and y
    yOffset; // int value to offset y values
    sandCounter; // count for how much sand has settled
    sandEntranceBlocked; // bool for whether sand has blocked the sand entrance
    hasAbyss; // whether there is an abyss below (otherwise there is a floor)

    constructor(sandStartLocation, hasAbyss = true) {
        this.grid = [];
        this.rockInstructions = [];
        this.lowestY = Infinity;
        this.highestY = 0;
        this.highestX = 0;
        this.sandStart = sandStartLocation;
        this.sandCounter = 0;
        this.sandEntranceBlocked = false;
        this.hasAbyss = hasAbyss;
    }

    // Parses the input into instruction list, finds important points, and then creates grid
    parseInput = () => {
        this.rockInstructions = this.rockInstructions.map(line => {
            return line.split(' -> ').map(pair => {
                return pair.split(',').map(number => parseInt(number));
            });
        });

        this.findLowestPoints();
        this.assembleGrid();
    }

    // Finds important points, or sets arbitrarily large ones if there's a floor
    findLowestPoints = () => {
        if (this.hasAbyss){
            this.rockInstructions.forEach(line => {
                line.forEach(pair => {
                    this.lowestY = this.lowestY > pair[0] ? pair[0] : this.lowestY;
                    this.highestY = this.highestY > pair[0] ? this.highestY : pair[0];
                    this.highestX = this.highestX > pair[1] ? this.highestX : pair[1];
                })
            })
            this.yOffset = this.lowestY - 1;
        } else {
            this.lowestY = 0;
            this.highestY = 999;
            this.yOffset = 0;
            this.rockInstructions.forEach(line => {
                line.forEach(pair => {
                    this.highestX = this.highestX > pair[1] ? this.highestX : pair[1];
                })
            })
        }
        
    }

    // Builds the grid
    assembleGrid = () => {
        const gridWidth = this.highestY - this.lowestY + 3; // 2 for space on left and right, 1 for inclusivity
        
        for (let x = 0; x < this.highestX + 1; x++){
            this.grid.push([]);
            if (this.hasAbyss){
                for (let y = 0; y < gridWidth; y++){
                    this.grid[x].push(new Node(x, y));
                }
            } else {
                for (let y = 0; y < 1000; y++){
                    this.grid[x].push(new Node(x, y));
                }
            }
        }
    }

    // Prints grid to console
    printGrid = () => {
        let printString = '';
        this.grid.forEach(line => {
            line.forEach(node => {
                switch (node.contents){
                    case AIR:
                        printString += '.';
                        break;
                    case ROCK:
                        printString += '#';
                        break;
                    case SAND:
                        printString += 'O';
                        break;
                    default:
                        printString += 'E';
                        break; // For error cases
                }
            })
            printString += '\n';
        })
        printString += '\n';
        console.log(printString);
    }

    // For each line, per set of pairs, draw backwards from one point to another
    drawRock = () => {
        const Y = 0;
        const X = 1;
        // For each instruction
        this.rockInstructions.forEach(line => {
            // Start with first and second pair, so start at 1
            for (let currentPair = 1; currentPair < line.length; currentPair++){
                let previousPair = currentPair -1;
                let currentX = line[currentPair][X];
                let currentY = line[currentPair][Y] - this.yOffset;
                let previousX = line[previousPair][X];
                let previousY = line[previousPair][Y] - this.yOffset;
                // Are we drawing up?
                if (currentX > previousX){
                    while (currentX >= previousX){
                        this.grid[currentX][currentY].contents = ROCK;
                        currentX--;
                    }
                }
                // Are we drawing down?
                else if (currentX < previousX){
                    while (currentX <= previousX){
                        this.grid[currentX][currentY].contents = ROCK;
                        currentX++;
                    }
                }
                // Are we drawing left?
                else if (currentY > previousY){
                    while (currentY >= previousY){
                        this.grid[currentX][currentY].contents = ROCK;
                        currentY--;
                    }
                }
                // Are we drawing right?
                else if (currentY < previousY){
                    while (currentY <= previousY){
                        this.grid[currentX][currentY].contents = ROCK;
                        currentY++;
                    }
                }
            }
        })
    }

    // Clears all sand from trap and resets sandCounter
    resetTrap = () => {
        for (let x = 0; x < this.grid.length; x++){
            for (let y = 0; y < this.grid[x].length; y++){
                if (this.grid[x][y].contents === SAND){
                    this.grid[x][y].contents = AIR;
                }
            }
        }
        this.sandCounter = 0;
    }

    // Starts the trap
    springTrap = () => {
        // If there's a floor, need to add two rows, with bottom row being all rock
        if (!this.hasAbyss){
            const gridWidth = this.highestY - this.lowestY + 3; // 2 for space on left and right, 1 for inclusivity
            const row1 = [];
            const row2 = [];
            for (let y = 0; y < gridWidth; y++){
                row1.push(new Node(this.grid.length, y));
                row2.push(new Node(this.grid.length + 1, y, ROCK));
            }
            this.grid.push(row1, row2);
            this.highestX +=2;
        }

        // Drop sand until either sand reaches abyss if present
        // or until the sand entrance is blocked
        while(this.dropSand(this.hasAbyss)){}
    }

    // Drops a single block of sand, stopping when settled.
    // Returns true to say "drop more", else false for "stop"
    dropSand = () => {
        // Copy of start location with offset
        let sand = {
            x: this.sandStart.x,
            y: this.sandStart.y - this.yOffset
        }; 
        
        let sandOutsideBounds = false;
        let sandStopped = false;

        // While sand hasn't stopped/settled
        while (!sandStopped){
            // Is sand within bounds?
            sandOutsideBounds = this.isSandOutsideBounds(sand.x, sand.y);
            if ((sandOutsideBounds && this.hasAbyss)){ 
                break;
            }
            // Check below
            else if (this.isAir(sand.x + 1, sand.y)){
                sand.x += 1;
            }
            // Check down to left
            else if (sand.y > 0 && this.isAir(sand.x + 1, sand.y - 1)){
                sand.x += 1;
                sand.y -= 1;
            }
            // Check down to right
            else if (sand.y < this.grid[sand.x].length - 1 && this.isAir(sand.x + 1, sand.y + 1)){
                sand.x += 1;
                sand.y += 1;
            }
            // Then sand is stopped
            else {
                this.grid[sand.x][sand.y].contents = SAND;
                sandStopped = true;
                this.sandCounter++;
                if (sand.x == this.sandStart.x && sand.y == this.sandStart.y - this.yOffset){
                    this.sandEntranceBlocked = true;
                }
            }
        }
        
        // If out of bounds, return false == stop dropping sand
        // Or if the entrance is blocked
        // Otherwise return true == drop more sand
        if ((sandOutsideBounds && this.hasAbyss) || this.sandEntranceBlocked){
            return false;
        } else {
            return true;
        }
    }

    // Checks if location contains AIR
    isAir = (x, y) => {
        return this.grid[x][y].contents === AIR ? true : false;
    }

    // Checks if location is out of bounds
    // Only useful when looking for the abyss
    isSandOutsideBounds = (x, y) => {
        if (
            x >= this.highestX ||
            y <= this.lowestY - this.yOffset ||
            y >= this.highestY - this.yOffset
        ){
            return true;
        }
        return false;
    }
}