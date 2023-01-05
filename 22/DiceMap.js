const [RIGHT, DOWN, LEFT, UP] = [0, 1, 2 ,3];

class Node{
    face;
    x;
    y;
    rock;
    up;
    down;
    left;
    right;
    xOffset;
    yOffset;

    constructor(face, x, y, rock, xOffset = 0, yOffset = 0){
        this.face = face;
        this.x = x;
        this.y = y;
        this.rock = rock;
        this.xOffset = xOffset;
        this.yOffset = yOffset;
    }
}

export default class DiceMap{
    input;
    instructions;
    nodes;
    cubeGrid;
    currentNode;
    currentDirection;

    constructor(){
        this.input = [];
        this.instructions = [];
        this.nodes = [];
    }

    followInstructions = (USE_TEST_DATA) => {
        this.currentNode = this.nodes[0][0][0]; // Start top left of face 0
        this.currentDirection = RIGHT;

        this.instructions.forEach(instruction => {
            // If it's a number, move
            if (parseInt(instruction)){
                move: for (let i = 0; i < instruction; i++){
                    switch (this.currentDirection) {
                        case UP:
                            if (!this.currentNode.up.rock) this.currentNode = this.currentNode.up;
                            else break move;
                            break;
                        case DOWN:
                            if (!this.currentNode.down.rock) this.currentNode = this.currentNode.down;
                            else break move;
                            break;
                        case LEFT:
                            if (!this.currentNode.left.rock) this.currentNode = this.currentNode.left;
                            else break move;
                            break;
                        case RIGHT:
                            if (!this.currentNode.right.rock) this.currentNode = this.currentNode.right;
                            else break move;
                            break;
                    }
                }
            // Otherwise it's a turning instruction, so turn
            } else {
                if (instruction == 'L'){
                    this.currentDirection -= 1;
                    if (this.currentDirection < 0) this.currentDirection = UP;
                } else {
                    this.currentDirection = (this.currentDirection + 1) % 4; // for 4 directions
                }
            }
        })
    }

    getLocationScore = () => {
        return (1000 * (this.currentNode.x + this.currentNode.xOffset + 1)) + (4 * (this.currentNode.y + this.currentNode.yOffset + 1)) + this.currentDirection;
    }

    createNodes = (USE_TEST_DATA, cube = false) => {
        // Trim last two lines of input
        const gridInput = [];
        for (let i = 0; i < this.input.length - 2; i++){
            gridInput.push(this.input[i].split(''));
        }

        // What is the length of each face?
        let faceLength = Infinity;
        gridInput.forEach((line, index) => {
            let lineLength = 0;
            gridInput[index].forEach(element => {
                if (element != ' ')
                    lineLength++;
            });
            if (lineLength < faceLength) faceLength = lineLength;
        })
        
        console.log(faceLength);
        // For each line in face 0
        const face0 = [];
        let xOffset = 0; // Identical
        let yOffset = USE_TEST_DATA ? faceLength * 2 : faceLength;
        for (let x = xOffset; x < xOffset + faceLength; x++){
            const row = [];
            for (let y = yOffset; y < yOffset + faceLength; y++){
                let rock = gridInput[x][y] == '#';
                row.push(new Node(0, x - xOffset, y - yOffset, rock, xOffset, yOffset));
            }
            face0.push(row);
        }
        // For each line in face 1
        const face1 = [];
        xOffset = USE_TEST_DATA ? faceLength : 0;
        yOffset = USE_TEST_DATA ? 0 : faceLength * 2;
        for (let x = xOffset; x < xOffset + faceLength; x++){
            const row = [];
            for (let y = yOffset; y < yOffset + faceLength; y++){
                let rock = gridInput[x][y] == '#';
                row.push(new Node(1, x - xOffset, y - yOffset, rock, xOffset, yOffset));
            }
            face1.push(row);
        }
        // For each line in face 2
        const face2 = [];
        xOffset = faceLength; // Both offsets are the same for both test and real data
        yOffset = faceLength;
        for (let x = xOffset; x < xOffset + faceLength; x++){
            const row = [];
            for (let y = yOffset; y < yOffset + faceLength; y++){
                let rock = gridInput[x][y] == '#';
                row.push(new Node(2, x - xOffset, y - yOffset, rock, xOffset, yOffset));
            }
            face2.push(row);
        }
        // For each line in face 3
        const face3 = [];
        xOffset = USE_TEST_DATA ? faceLength : faceLength * 2;
        yOffset = USE_TEST_DATA ? faceLength * 2 : 0;
        for (let x = xOffset; x < xOffset + faceLength; x++){
            const row = [];
            for (let y = yOffset; y < yOffset + faceLength; y++){
                let rock = gridInput[x][y] == '#';
                row.push(new Node(3, x - xOffset, y - yOffset, rock, xOffset, yOffset));
            }
            face3.push(row);
        }
        // For each line in face 4
        const face4 = [];
        xOffset = faceLength * 2; // Identical
        yOffset = USE_TEST_DATA ? faceLength * 2 : faceLength;
        for (let x = xOffset; x < xOffset + faceLength; x++){
            const row = [];
            for (let y = yOffset; y < yOffset + faceLength; y++){
                let rock = gridInput[x][y] == '#';
                row.push(new Node(4, x - xOffset, y - yOffset, rock, xOffset, yOffset));
            }
            face4.push(row);
        }
        // For each line in face 5
        const face5 = [];
        xOffset = USE_TEST_DATA ? faceLength * 2 : faceLength * 3;
        yOffset = USE_TEST_DATA ? faceLength * 3 : 0;
        for (let x = xOffset; x < xOffset + faceLength; x++){
            const row = [];
            for (let y = yOffset; y < yOffset + faceLength; y++){
                let rock = gridInput[x][y] == '#';
                row.push(new Node(5, x - xOffset, y - yOffset, rock, xOffset, yOffset));
            }
            face5.push(row);
        }

        this.nodes.push(face0, face1, face2, face3, face4, face5);

        // Connect interior nodes to surrounding nodes on face
        this.nodes.forEach(face => {
            face.forEach(row => {
                row.forEach(node => {
                    // Conditions to determine interior nodes only
                    if (this.isInteriorNode(node, faceLength)){
                        node.up = this.nodes[node.face][node.x - 1][node.y];
                        node.down = this.nodes[node.face][node.x + 1][node.y];
                        node.left = this.nodes[node.face][node.x][node.y - 1];
                        node.right = this.nodes[node.face][node.x][node.y + 1];
                    } else {
                        // Connect edge nodes according to flat or cube shape
                        if (this.cubeGrid){

                        } else {
                            this.connectEdges(node, faceLength, USE_TEST_DATA);
                        }
                    }
                })
            })
        })
    }

    connectEdges = (node, faceLength, USE_TEST_DATA) => {
        let faceRelations = this.getFaceRelations(node.face, USE_TEST_DATA);
        node.up = node.x == 0 ? this.nodes[faceRelations.up][faceLength - 1][node.y] : this.nodes[node.face][node.x - 1][node.y];
        node.down = node.x == faceLength - 1 ? this.nodes[faceRelations.down][0][node.y] : this.nodes[node.face][node.x + 1][node.y];
        node.left = node.y == 0 ? this.nodes[faceRelations.left][node.x][faceLength - 1] : this.nodes[node.face][node.x][node.y - 1];
        node.right = node.y == faceLength - 1 ? this.nodes[faceRelations.right][node.x][0] : this.nodes[node.face][node.x][node.y + 1];
    }

    getFaceRelations = (face, USE_TEST_DATA) => {
        if (USE_TEST_DATA){
            switch (face) {
                case 0:
                    return {up: 4, down: 3, left: 0, right: 0};
                case 1:
                    return {up: 1, down: 1, left: 3, right: 2};
                case 2:
                    return {up: 2, down: 2, left: 1, right: 3};
                case 3:
                    return {up: 0, down: 4, left: 2, right: 1};
                case 4:
                    return {up: 3, down: 0, left: 5, right: 5};
                case 5:
                    return {up: 5, down: 5, left: 4, right: 4};
            }
        } else {
            switch (face) {
                case 0:
                    return {up: 4, down: 2, left: 1, right: 1};
                case 1:
                    return {up: 1, down: 1, left: 0, right: 0};
                case 2:
                    return {up: 0, down: 4, left: 2, right: 2};
                case 3:
                    return {up: 5, down: 5, left: 4, right: 4};
                case 4:
                    return {up: 2, down: 0, left: 3, right: 3};
                case 5:
                    return {up: 3, down: 3, left: 5, right: 5};
            }
        }
        
    }

    isInteriorNode = (node, faceLength) => {
        return (
            node.x > 0 &&
            node.y > 0 &&
            node.x < faceLength - 1 &&
            node.y < faceLength - 1
        ) 
    }

    convertInputToInstructions = () => {
        const instructionLine = this.input[this.input.length - 1].split('');
        let index = 0;
        let currentCharacter = instructionLine[index]; // Track current character
        let runningNumber = ''; // Store string numbers
        // While there are valid characters incoming
        while (currentCharacter){
            // If it can't be parsed (letter) and not a 0
            if (!parseInt(currentCharacter) && parseInt(currentCharacter) !== 0){
                this.instructions.push(parseInt(runningNumber)); // convert and push number
                this.instructions.push(currentCharacter); // push direction
                runningNumber = ''; // reset
            } else {
                runningNumber += currentCharacter; // build up the number more
            }
            index++;
            currentCharacter = instructionLine[index]; // next character
        }
        this.instructions.push(parseInt(runningNumber)); // get that last number on there.
    }
}