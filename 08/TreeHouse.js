export default class TreeHouse{
    trees;
    largestScenicScore;
    tempViewRange;

    constructor(){
        this.trees = [];
        this.tempViewRange = this.resetViewCounts();
        this.largestScenicScore = 0;
    }

    splitData = () => {
        this.trees = this.trees.map(row => row.split('').map(tree => parseInt(tree)));
    }

    resetViewCounts = () => {
        return {
            north: 0,
            south: 0,
            east: 0,
            west: 0
        }
    }

    calculateScenicScore = () => {
        let score = 1;
        Object.values(this.tempViewRange).forEach(value => {score *= value;});
        return score;
    }

    isTreeVisible = (xCoord, yCoord) => {
        let height = this.trees[xCoord][yCoord];
        this.tempViewRange = this.resetViewCounts();
        if (
            this.visibleFromEast(xCoord, yCoord, height) ||
            this.visibleFromWest(xCoord, yCoord, height) ||
            this.visibleFromNorth(xCoord, yCoord, height) ||
            this.visibleFromSouth(xCoord, yCoord, height)
        )
            return true;
        console.log(this.tempViewRange);
        const currentScenicScore = this.calculateScenicScore();
        this.largestScenicScore = currentScenicScore > this.largestScenicScore ? currentScenicScore : this.largestScenicScore;
        return false;
    }

     visibleFromEast = (xCoord, yCoord, height) => { 
        if (yCoord == this.trees[xCoord].length - 1)
            return true;
        if (this.trees[xCoord][yCoord + 1] >= height){
            this.tempViewRange.east++;
            return false;
        }
        this.tempViewRange.east++;
        return this.visibleFromEast(xCoord, yCoord + 1, height);
    }

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

    countVisibleTrees = () => {
        let count = 0;
        for (let x = 0; x < this.trees.length; x++){
            for (let y = 0; y < this.trees[x].length; y++){
                console.log(this.trees[x][y], this.isTreeVisible(x, y))
                if (this.isTreeVisible(x, y))
                    count++;
                
            }
        }
        return count;
    }
}