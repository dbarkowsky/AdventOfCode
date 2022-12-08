export default class TreeHouse{
    trees;

    constructor(){
        this.trees = [];
    }

    splitData = () => {
        this.trees = this.trees.map(row => row.split('').map(tree => parseInt(tree)));
    }

    isTreeVisible = (xCoord, yCoord, originalHeight = false) => {
        let height = this.trees[xCoord][yCoord];
        if (originalHeight)
            height = originalHeight;

        if (
            xCoord == 0 ||
            xCoord == this.trees.length - 1 ||
            yCoord == 0 ||
            yCoord == this.trees[xCoord].length - 1
        ){
            return true;
        } else if (
            (this.trees[xCoord - 1][yCoord] < height && this.isTreeVisible(xCoord - 1, yCoord, height)) ||
            (this.trees[xCoord + 1][yCoord] < height && this.isTreeVisible(xCoord + 1, yCoord, height)) ||
            (this.trees[xCoord][yCoord - 1] < height && this.isTreeVisible(xCoord, yCoord - 1, height)) ||
            (this.trees[xCoord][yCoord + 1] < height && this.isTreeVisible(xCoord, yCoord + 1, height))  
        ){
            return true;
        }

        return false;
    }

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