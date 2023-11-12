const [bUp, bDown, bLeft, bRight] = ['^', 'v', '<', '>'];
const WALL = '#';

export default class Blizzard{
    walls; // Set of wall coordinates
    blizzards; // List of blizzard objects
    blizzardLocations; // Set of blizzard-occupied spaces
    rawInput;
    goal; // Standard ending location

    constructor(){
        this.walls = new Set();
        this.blizzardLocations = new Set();
        this.blizzards = [];
        this.rawInput = [];
        this.goal = {};
    }

    // Converts raw input into blizzards and walls
    parseInput = () => {
        // Reset old
        this.walls = new Set();
        this.blizzardLocations = new Set();
        this.blizzards = [];

        // Convert input to walls and blizzards
        this.rawInput.forEach((line, x) => {
            line.split('').forEach((point, y) => {
                switch (point) {
                    case WALL:
                        this.walls.add(`${x},${y}`);
                        break;
                    case bUp:
                    case bDown: 
                    case bLeft: 
                    case bRight:
                        this.blizzardLocations.add(`${x},${y}`);
                        this.blizzards.push({type: point, x, y});
                        break;
                    default:
                        break;
                }     
                this.goal.y = y - 1;
            })
            this.goal.x  = x;
        });
        // Add extra wall at top to prevent going that direction
        this.walls.add('-1,1');
        // Add extra wall below to prevent similar issue if returning
        this.walls.add(`${this.goal.x + 1},${this.goal.y}`);
    }

    // Runs the maze from start to goal. Does not reset blizzards, so can be run back to back.
    runMaze = (startingPosition = {x: 0, y: 1}, goal = this.goal) => {
        // Start queue with this position
        let routeQueue = [startingPosition];
        // Until end is found
        let turns = 0;
        while (true){
            // While queue has possibilities
            let currentRouteQueueLength = routeQueue.length;
            let nextQueue = [];
            for (let i = 0; i < currentRouteQueueLength; i++){
                let currentRoute = routeQueue.shift();
                // Check if this position == the goal
                    // if so, return count of turns
                if (currentRoute.x == goal.x && currentRoute.y == goal.y){
                    return turns;
                }

                // Otherwise, if it encountered a blizzard, end path
                if (this.blizzardLocations.has(`${currentRoute.x},${currentRoute.y}`)){
                    continue;
                }

                // If it didn't encounter blizzard, add all possible paths to queue
                // ALSO! Don't let it go back to start position
                // Say in place
                if (!this.routeQueueHas(nextQueue, currentRoute))
                    nextQueue.push({x: currentRoute.x, y: currentRoute.y});
                // North
                if (!this.walls.has(`${currentRoute.x - 1},${currentRoute.y}`) && !this.routeQueueHas(nextQueue, {x: currentRoute.x - 1, y: currentRoute.y})) 
                    nextQueue.push({x: currentRoute.x - 1, y: currentRoute.y});
                // South
                if (!this.walls.has(`${currentRoute.x + 1},${currentRoute.y}`) && !this.routeQueueHas(nextQueue, {x: currentRoute.x + 1, y: currentRoute.y})) 
                    nextQueue.push({x: currentRoute.x + 1, y: currentRoute.y});
                // West
                if (!this.walls.has(`${currentRoute.x},${currentRoute.y - 1}`) && !this.routeQueueHas(nextQueue, {x: currentRoute.x, y: currentRoute.y - 1})) 
                    nextQueue.push({x: currentRoute.x, y: currentRoute.y - 1});
                // East
                if (!this.walls.has(`${currentRoute.x},${currentRoute.y + 1}`) && !this.routeQueueHas(nextQueue, {x: currentRoute.x, y: currentRoute.y + 1})) 
                    nextQueue.push({x: currentRoute.x, y: currentRoute.y + 1});
            }
            routeQueue = nextQueue;
            // Simulate one minute passing for blizzard movement
            this.updateBlizzards();
            turns++;
        }
    }

    // Checks if the current queue being built already has this value
    // Saves a lot of time!
    routeQueueHas = (queue, location) => {
        return queue.find(el => el.y == location.y && el.x == location.x);
    }

    // Check if location is the starting location. Didn't end up being needed.
    isNotStart = (point) => {
        return !(point.x == 0 && point.y == 1);
    }

    // Moves all the blizzards one space, wrapping if needed
    updateBlizzards = () => {
        this.blizzardLocations = new Set(); // reset
        this.blizzards.forEach(blizzard => {
            switch (blizzard.type) {
                case bUp:
                    if (this.walls.has(`${blizzard.x - 1},${blizzard.y}`)){
                        // Place at opposite end
                        blizzard.x = this.goal.x - 1; // One row above bottom
                    } else {
                        // Advance one
                        blizzard.x--;
                    }
                    break;
                case bDown:
                    if (this.walls.has(`${blizzard.x + 1},${blizzard.y}`)){
                        // Place at opposite end
                        blizzard.x = 1; // First viable row
                    } else {
                        // Advance one
                        blizzard.x++;
                    }
                    break;
                case bLeft:
                    if (this.walls.has(`${blizzard.x},${blizzard.y - 1}`)){
                        // Place at opposite end
                        blizzard.y = this.goal.y;
                    } else {
                        // Advance one
                        blizzard.y--;
                    }
                    break;
                case bRight:
                    if (this.walls.has(`${blizzard.x},${blizzard.y + 1}`)){
                        // Place at opposite end
                        blizzard.y = 1;
                    } else {
                        // Advance one
                        blizzard.y++;
                    }
                    break;
            }
            // Add to blizzard set
            this.blizzardLocations.add(`${blizzard.x},${blizzard.y}`);
        })
    }

    // Get total number of turns going from start to end to start to end again.
    thereAndBackAndThereAgain = () => {
        const start = {x: 0, y: 1};
        const end = {x: this.goal.x, y: this.goal.y};

        const there = this.runMaze(start, end);
        const back = this.runMaze(end, start);
        const thereAgain = this.runMaze(start, end);

        return there + back + thereAgain;
    }
}