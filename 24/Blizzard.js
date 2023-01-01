const [bUp, bDown, bLeft, bRight] = ['^', 'v', '<', '>'];
const WALL = '#';

export default class Blizzard{
    walls; // Set of wall coordinates
    blizzards; // List of blizzard objects
    blizzardLocations; // Set of blizzard-occupied spaces
    rawInput;
    goal;

    constructor(){
        this.walls = new Set();
        this.blizzardLocations = new Set();
        this.blizzards = [];
        this.rawInput = [];
        this.goal = {};
    }

    parseInput = () => {
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
    }

    runMaze = () => {
        // Set initial position
        let startingPosition = {x: 0, y: 1};
        // Start queue with this position
        const routeQueue = [startingPosition];
        // Until end is found
        let turns = 0;
        while (true){
            // While queue has possibilities
            while (routeQueue.length > 0){
                let currentRoute = routeQueue.shift();
                // Check if this position == the goal
                    // if so, return count of turns
                if (currentRoute.x == this.goal.x && currentRoute.y == this.goal.y){
                    return turns;
                }

                // Otherwise, if it encountered a blizzard, end path
                if (this.blizzardLocations.has(`${currentRoute.x},${currentRoute.y}`)){
                    continue;
                }

                // If it didn't encounter blizzard, add all possible paths to queue
                // ALSO! Don't let it go back to start position
                // Say in place
                if (this.isNotStart(currentRoute))
                    routeQueue.push({x: currentRoute.x, y: currentRoute.y});
                // North
                if (!this.walls.has(`${currentRoute.x - 1},${currentRoute.y}`)) 
                    routeQueue.push({x: currentRoute.x - 1, y: currentRoute.y});
                // South
                if (!this.walls.has(`${currentRoute.x + 1},${currentRoute.y}`)) 
                    routeQueue.push({x: currentRoute.x + 1, y: currentRoute.y});
                // West
                if (!this.walls.has(`${currentRoute.x},${currentRoute.y - 1}`)) 
                    routeQueue.push({x: currentRoute.x, y: currentRoute.y - 1});
                // East
                if (!this.walls.has(`${currentRoute.x},${currentRoute.y + 1}`)) 
                    routeQueue.push({x: currentRoute.x, y: currentRoute.y + 1});
            }

            // Simulate one minute passing for blizzard movement
            this.updateBlizzards();
            turns++;
        }
    }

    isNotStart = (point) => {
        return !(point.x == 0 && point.y == 1);
    }

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
            this.blizzardLocations.add(`${blizzard.x},${blizzard.y}`);
        })
    }
}