class HeightNode{
    height; // int converted from char value
    importance; // Either E (end), S (start), or undefined
    visited; // bool, so we don't keep visiting the same node
    x; // x coordinate
    y; // y coordinate
    distanceFromStart;  // Node's distance from starting node
    
    constructor(height, xCoord, yCoord){
        // Is this node of importance?
        if (height == 'S'.charCodeAt(0)){
            this.importance = 'S';
            this.height = 'a'.charCodeAt(0);
        }
        else if (height == 'E'.charCodeAt(0)){
            this.importance = 'E';
            this.height = 'z'.charCodeAt(0);
        } else {
            this.height = height;
        }
        
        this.visited = false;
        this.x = xCoord;
        this.y = yCoord;
        this.distanceFromStart = 0;
    }
}

export default class Heightmap{
    map; // 2D list of Heightnodes
    startingNode; // Pointer to starting node
    endingNode; // Pointer to ending node

    constructor(){
        this.map = [];
    }

    // Finds the path length (int) from starting node to ending node
    findPathLength = (startingNode) => {
        // Setup
        this.findEndingNode();
        startingNode.visited = true;
        startingNode.distanceFromStart = 0;
        const neighbouringNodes = [];
        neighbouringNodes.push(startingNode);
        let currentNode;
        let reachedEnd = false;

        // While there are neighbouring nodes still to check
        while (neighbouringNodes.length > 0){
            // Get the first node
            currentNode = neighbouringNodes.shift();
            // If it's the ending node, we're done
            if (currentNode.importance == this.endingNode.importance){
                reachedEnd = true;
                break;
            } else {
                // Get the node's surrounding neighbours
                const nextNeighbours = this.getValidAdjacentNodes(currentNode);
                // Mark each as visited, set distance from start, and add to queue
                nextNeighbours.forEach(neighbour => {
                    neighbour.visited = true;
                    neighbour.distanceFromStart = currentNode.distanceFromStart + 1;
                    neighbouringNodes.push(neighbour);
                });
            }
        }

        // To avoid returning a value when there was no path to the end
        if (reachedEnd)
            return currentNode.distanceFromStart;
    }

    // Gets neighbouring nodes based on three conditions (index, visited, height)
    getValidAdjacentNodes = (currentNode) => {
        const tempList = [];
        if (
            currentNode.x > 0 &&
            this.getNorth(currentNode).visited == false &&
            this.getNorth(currentNode).height - currentNode.height <= 1
        ){
            tempList.push(this.getNorth(currentNode));
        }

        if (
            currentNode.x < this.map.length - 1 &&
            this.getSouth(currentNode).visited == false &&
            this.getSouth(currentNode).height - currentNode.height <= 1
        ){
            tempList.push(this.getSouth(currentNode));
        }

        if (
            currentNode.y > 0 &&
            this.getWest(currentNode).visited == false &&
            this.getWest(currentNode).height - currentNode.height <= 1
        ){
            tempList.push(this.getWest(currentNode));
        }

        if (
            currentNode.y < this.map[currentNode.x].length - 1 &&
            this.getEast(currentNode).visited == false &&
            this.getEast(currentNode).height - currentNode.height <= 1
        ){
            tempList.push(this.getEast(currentNode));
        }
        
        return tempList;
    }

    // Finds the shortest path, comparing all nodes with 'a' height
    findShortestAPath = () => {
        const aNodes = this.getAllANodes();
        const aPathLengths = [];
        aNodes.forEach(node => {
            this.resetAllNodes(); // Must be false again
            let length = this.findPathLength(node);
            // Only add if path was found
            if (length){
                aPathLengths.push(length);
            }
        })
        // Get minimum path length
        return aPathLengths.reduce((min, current) => Math.min(min, current), Infinity);
    }

    // Returns list of nodes with 'a' height
    getAllANodes = () => {
        const aNodes = [];
        for (let row = 0; row < this.map.length; row++){
            for (let node = 0; node < this.map[row].length; node++){
                if (this.map[row][node].height == 'a'.charCodeAt(0)){
                    aNodes.push(this.map[row][node]);
                }
            }
        }
        return aNodes;
    }

    // Sets all nodes back to false
    resetAllNodes = () => {
        for (let row = 0; row < this.map.length; row++){
            for (let node = 0; node < this.map[row].length; node++){
                this.map[row][node].visited = false;
            }
        }
    }

    // Finds and returns the node with S importance
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

    // Finds and returns the node with E importance
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

    // Splits data into lists of nodes
    splitMapRows = () => {
        this.map = this.map.map(row => row.split(''));
        // Turn into nodes
        for (let row = 0; row < this.map.length; row++){
            for (let node = 0; node < this.map[row].length; node++){
                this.map[row][node] = new HeightNode(this.map[row][node].charCodeAt(0), row, node);
            }
        }
    }

    /*** Last four methods get neighbouring nodes ***/
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