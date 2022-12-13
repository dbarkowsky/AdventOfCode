class HeightNode{
    height;
    importance;
    visited;
    x;
    y;
    
    constructor(height, xCoord, yCoord){
        if (height == 'S'){
            this.importance = 'S';
            this.height = 'a';
        }
        else if (height == 'E'){
            this.importance = 'E';
            this.height = 'z';
        } else {
            this.height = height;
        }
        
        this.visited = false;
        this.x = xCoord;
        this.y = yCoord;
    }
}

export default class Heightmap{
    map;
    startingNode;
    endingNode;

    constructor(){
        this.map = [];
    }

    findPath = (currentNode) => {
        let returnValue = -1;
        if (currentNode){
            if (currentNode.importance == this.endingNode.importance){
                return 0;
            } else {
                // See if directions are either undefined or visited
                if (
                    this.getNorth(currentNode) &&
                    this.getNorth(currentNode).visited != true &&
                    (this.getNorth(currentNode).height == currentNode.height ||
                    this.getNorth(currentNode).height == currentNode.height + 1)
                ){
                    returnValue = 1 + this.findPath(this.getNorth(currentNode));
                    if (returnValue == 0){
                        return;
                    }
                }
            }
        }
    }

    findPathOrig = (currentNode) => {
        currentNode.visited = true;
        // Assuming S and E aren't the same node.
        if (currentNode.importance == 'E'){
            return 1;
        }
        
        let [north, south, east, west] = [-1, -1, -1, -1];
        let directionList = [];

        if (
            currentNode.x > 0 &&
            this.getNorth(currentNode).visited == false &&
            (this.getNorth(currentNode).height == currentNode.height ||
            this.getNorth(currentNode).height == currentNode.height + 1)
            
        ){
            north = 1 + this.findPath(this.getNorth(currentNode));
        }

        if (
            currentNode.x < this.map.length - 2 &&
            this.getSouth(currentNode).visited == false &&
            (this.getSouth(currentNode).height == currentNode.height ||
            this.getSouth(currentNode).height == currentNode.height + 1)
            
        ){
            south = 1 + this.findPath(this.getSouth(currentNode));
        }
        
        if (
            currentNode.y > 0 &&
            this.getWest(currentNode).visited == false &&
            (this.getWest(currentNode).height == currentNode.height ||
            this.getWest(currentNode).height == currentNode.height + 1)
        ){
            west = 1 + this.findPath(this.getWest(currentNode));
        }

        if (
            currentNode.y < this.map[currentNode.x].length - 2 &&
            this.getEast(currentNode).visited == false &&
            (this.getEast(currentNode).height == currentNode.height ||
            this.getEast(currentNode).height == currentNode.height + 1)
        ){
            east = 1 + this.findPath(this.getEast(currentNode));
        }
        
        directionList.push(north, south, east, west);
        directionList = directionList.filter(value => value >= 0).sort();

        return directionList[0];
    }

    findStartingNode = () => {
        for (let row = 0; row < this.map.length; row++){
            for (let node = 0; node < this.map[row].length; node++){
                if (this.map[row][node].importance == 'S'){
                    this.startingNode = this.map[row][node];
                    return;
                }
            }
        }
    }

    findEndingNode = () => {
        for (let row = 0; row < this.map.length; row++){
            for (let node = 0; node < this.map[row].length; node++){
                if (this.map[row][node].importance == 'E'){
                    this.endingNode = this.map[row][node];
                    return;
                }
            }
        }
    }

    splitMapRows = () => {
        this.map = this.map.map(row => row.split(''));
        // Turn into nodes
        for (let row = 0; row < this.map.length; row++){
            for (let node = 0; node < this.map[row].length; node++){
                this.map[row][node] = new HeightNode(this.map[row][node], row, node);
            }
        }
    }

    getNorth = (node) => {
        return this.map[node.x - 1][node.y];
    }

    getSouth = (node) => {
        return this.map[node.x + 1][node.y];
    }

    getWest = (node) => {
        return this.map[node.x][node.y - 1];
    }

    getEast = (node) => {
        return this.map[node.x][node.y + 1];
    }
}