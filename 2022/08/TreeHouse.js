export default class TreeHouse{
    trees; // 2D array of tree heights
    largestScenicScore; // int, largest scenic score recorded
    tempViewRange; // temporary object to track heights

    constructor(){
        this.trees = [];
        this.tempViewRange = this.resetViewCounts();
        this.largestScenicScore = 0;
    }

    // Splits rows into more arrays
    splitData = () => {
        this.trees = this.trees.map(row => row.split('').map(tree => parseInt(tree)));
    }

    // Returns default view count object.
    resetViewCounts = () => {
        return {
            north: 0,
            south: 0,
            east: 0,
            west: 0
        }
    }

    // Calculates scenic score based on view counts in tempViewRange
    calculateScenicScore = () => {
        let score = 1;
        Object.values(this.tempViewRange).forEach(value => {score *= value;});
        return score;
    }

    // Checks if a tree is visible from outside the forest. 
    // Also works to find largest scenic score
    isTreeVisible = (xCoord, yCoord) => {
        // Setup
        let height = this.trees[xCoord][yCoord];
        this.tempViewRange = this.resetViewCounts();

        // Determine visibility from all sides
        // Previously had this in a conditional statement for shortcutting, but needs to run all to calculate score properly.
        const isVisible ={
            north: this.visibleFromNorth(xCoord, yCoord, height),
            south: this.visibleFromSouth(xCoord, yCoord, height),
            east: this.visibleFromEast(xCoord, yCoord, height),
            west: this.visibleFromWest(xCoord, yCoord, height)
        }
        // console.log(`[${xCoord}, ${yCoord}]`, height, this.tempViewRange, this.calculateScenicScore());

        // Calculate scenic score for this tree and compare against previous largest.
        const currentScenicScore = this.calculateScenicScore();
        this.largestScenicScore = currentScenicScore > this.largestScenicScore ? currentScenicScore : this.largestScenicScore;
        
        // if visible from any side...
        if (Object.values(isVisible).includes(true))
            return true;
        return false;
    }

    // Checks visibility from East, adding to view range where needed.
    visibleFromEast = (xCoord, yCoord, height) => {
        // At edge of forest 
        if (yCoord == this.trees[xCoord].length - 1)
            return true;
        // Next tree is bigger or equal in height
        if (this.trees[xCoord][yCoord + 1] >= height){
            this.tempViewRange.east++;
            return false;
        }
        // Next tree is smaller
        this.tempViewRange.east++;
        return this.visibleFromEast(xCoord, yCoord + 1, height);
    }

    // Checks visibility from West, adding to view range where needed.
    visibleFromWest = (xCoord, yCoord, height) => {
        if (yCoord == 0)
            return true;
        if (this.trees[xCoord][yCoord - 1] >= height){
            this.tempViewRange.west++;
            return false;
        }
        this.tempViewRange.west++;
        return this.visibleFromWest(xCoord, yCoord - 1, height);
    }

    // Checks visibility from North, adding to view range where needed.
    visibleFromNorth = (xCoord, yCoord, height) => {
        if (xCoord == 0)
            return true;
        if (this.trees[xCoord - 1][yCoord] >= height){
            this.tempViewRange.north++;
            return false;
        }
        this.tempViewRange.north++;
        return this.visibleFromNorth(xCoord - 1, yCoord, height);
    }

    // Checks visibility from South, adding to view range where needed.
    visibleFromSouth = (xCoord, yCoord, height) => {
        if (xCoord == this.trees.length - 1)
            return true;
        if (this.trees[xCoord + 1][yCoord] >= height){
            this.tempViewRange.south++;
            return false;
        }
        this.tempViewRange.south++;
        return this.visibleFromSouth(xCoord + 1, yCoord, height);
    }

    // For each tree in forest, see if it is visible and add to total count of visible trees.
    countVisibleTrees = () => {
        let count = 0;
        for (let x = 0; x < this.trees.length; x++){
            for (let y = 0; y < this.trees[x].length; y++){
                if (this.isTreeVisible(x, y))
                    count++;
                
            }
        }
        return count;
    }
}