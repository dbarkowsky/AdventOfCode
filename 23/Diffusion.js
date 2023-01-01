class Elf{
    x;
    y;
    pX;
    pY;

    constructor(x, y){
        this.x = x;
        this.y = y;
    }

    toString = () => `${this.x},${this.y}`;

    proposedString = () => `${this.pX},${this.pY}`;
}

const [NORTH, SOUTH, WEST, EAST] = [0, 1, 2, 3];

export default class Diffusion{
    currentLocations; // Set of current elf locations
    proposedLocations; // Object of all proposed locations and their counts
    elves; // List to hold all elf nodes
    rawInput;
    direction; // 0 = N, 1 = S, 2 = W, 3 = E
    noElfMoved; // boolean for part 2

    constructor(){
        this.currentLocations = new Set();
        this.proposedLocations = {};
        this.elves = [];
        this.rawInput = [];
        this.direction = NORTH;
        this.noElfMoved = false;
    }

    // Converts input to elves and adds to current location set
    parseInput = () => {
        this.rawInput.forEach((line, x) => {
            line.split('').forEach((position, y) => {
                if (position === '#'){
                    // Create elf
                    let elf = new Elf(x, y);
                    this.elves.push(elf);
                    // Add their position to the currentLocations set
                    this.currentLocations.add(elf.toString());
                }
            })
        })
    }

    // A single round of diffusing the elves' positions
    // They pick a place to move, then try to move there.
    diffuse = () => {
        // Reset elf checker
        this.noElfMoved = true;
        // Determine proposed locations for elves
        this.proposedLocations = {}; // reset
        this.elves.forEach(elf => {
            // Get all surrounding locations, bools
            const occupiedLocations = {
                nw: this.currentLocations.has(`${elf.x-1},${elf.y-1}`),
                n: this.currentLocations.has(`${elf.x-1},${elf.y}`),
                ne: this.currentLocations.has(`${elf.x-1},${elf.y+1}`),
                e: this.currentLocations.has(`${elf.x},${elf.y+1}`),
                w: this.currentLocations.has(`${elf.x},${elf.y-1}`),
                sw: this.currentLocations.has(`${elf.x+1},${elf.y-1}`),
                s: this.currentLocations.has(`${elf.x+1},${elf.y}`),
                se: this.currentLocations.has(`${elf.x+1},${elf.y+1}`) 
            }

            // If all locations are empty
            if (Object.values(occupiedLocations).every(value => !value)){
                // Don't move
                elf.pX = elf.x;
                elf.pY = elf.y;
            } else {
                this.noElfMoved = false;
                let directionDetermined = false;
                let currentDirection = this.direction;
                findDirection : for (let i = 0; i < 4; i++){
                    if (this.checkDirection(currentDirection, occupiedLocations)){
                        this.setProposed(elf, currentDirection);
                        directionDetermined = true;
                        break findDirection;
                    } else {
                        currentDirection = (currentDirection + 1) % 4;
                    }
                }
                // If no valid moves, stay put?
                if (!directionDetermined){
                    elf.pX = elf.x;
                    elf.pY = elf.y;
                }
            }

            // Add proposed locations to obj
            this.proposedLocations[elf.proposedString()] = this.proposedLocations[elf.proposedString()] 
                                                    ? this.proposedLocations[elf.proposedString()] + 1
                                                    : 1;

        })

        // Determine next starting direction
        this.direction = (this.direction + 1) % 4;

        // Reset current locations
        this.currentLocations = new Set();
        // Elves try to move if no other elves want to move there
        this.elves.forEach(elf => {
            // If only one elf wanted to move there... update location
            if (this.proposedLocations[elf.proposedString()] == 1){
                elf.x = elf.pX;
                elf.y = elf.pY;
            }
            this.currentLocations.add(elf.toString());
        });
    }

    // Sets the elf's proposed movement variables
    setProposed = (elf, direction) => {
        switch (direction) {
            case NORTH:
                elf.pX = elf.x - 1;
                elf.pY = elf.y;
                break;
            case SOUTH:
                elf.pX = elf.x + 1;
                elf.pY = elf.y;
                break;
            case WEST:
                elf.pX = elf.x;
                elf.pY = elf.y - 1;
                break;
            case EAST:
                elf.pX = elf.x;
                elf.pY = elf.y + 1;
                break;
        }
    }

    // Checks if a direction is valid to move to
    checkDirection = (direction, occupiedLocations) => {
        switch (direction) {
            case NORTH:
                if (
                    !occupiedLocations.nw &&
                    !occupiedLocations.n &&
                    !occupiedLocations.ne
                ){
                    return true;
                }
                break;
            case SOUTH:
                if (
                    !occupiedLocations.sw &&
                    !occupiedLocations.s &&
                    !occupiedLocations.se
                ){
                    return true;
                }
                break;
            case WEST:
                if (
                    !occupiedLocations.nw &&
                    !occupiedLocations.w &&
                    !occupiedLocations.sw
                ){
                    return true;
                }
                break;
            case EAST:
                if (
                    !occupiedLocations.ne &&
                    !occupiedLocations.e &&
                    !occupiedLocations.se
                ){
                    return true;
                }
                break;
        }
        return false;
    }
    
    // Runs for a specified number of rounds
    runRounds = (rounds) => {
        for (let i = 0; i < rounds; i++){
            this.diffuse();
        }
    }

    // Runs until elves are fully spaced out, returns number of rounds
    runTillDone = () => {
        let counter = 0;
        while (!this.noElfMoved){
            this.diffuse();
            counter++;
        }
        return counter;
    }

    // Gets all the empty spaces between elves not occupied
    getEmptySpaces = () => {
        // Get total size of grid...
        let minX = Infinity;
        let maxX = 0;
        let minY = Infinity;
        let maxY = 0;

        this.elves.forEach(elf => {
            minX = elf.x < minX ? elf.x : minX;
            maxX = elf.x > maxX ? elf.x : maxX;
            minY = elf.y < minY ? elf.y : minY;
            maxY = elf.y > maxY ? elf.y : maxY;
        })

        const width = maxY - minY + 1;
        const height = maxX - minX + 1;

        const area = width * height;

        // Return area - the number of elves
        return area - this.elves.length;
    }
}