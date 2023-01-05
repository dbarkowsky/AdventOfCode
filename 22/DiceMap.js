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
                    let initialFace = this.currentNode.face;
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
                    // If we wrap the side of a cube... what direction should we be going now?
                    if (initialFace != this.currentNode.face && this.cubeGrid){
                        this.currentDirection = this.determineNewDirection(initialFace, this.currentNode.face, USE_TEST_DATA);
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

    determineNewDirection = (initialFace, currentFace, USE_TEST_DATA) => {
        if (USE_TEST_DATA){
            switch(initialFace){
                case 0:
                    switch (currentFace) {
                        case 1:
                        case 2:
                        case 3:
                            return DOWN;
                        case 5:
                            return LEFT;
                    }
                case 1:
                    switch (currentFace) {
                        case 0:
                            return DOWN;
                        case 2:
                            return RIGHT;
                        case 4:
                            return UP;
                        case 5:
                            return DOWN;
                    }
                case 2:
                    switch (currentFace) {
                        case 0:
                            return RIGHT;
                        case 1:
                            return LEFT;
                        case 3:
                        case 4:
                            return RIGHT;
                    }
                case 3:
                    switch (currentFace) {
                        case 0:
                            return UP;
                        case 2:
                            return LEFT;
                        case 4:
                        case 5:
                            return DOWN;
                    }
                case 4:
                    switch (currentFace) {
                        case 1:  
                        case 2:  
                        case 3:
                            return UP;
                        case 5:
                            return RIGHT;
                    }
                case 5:
                    switch (currentFace) {
                        case 0:  
                            return LEFT;
                        case 1:  
                            return RIGHT;
                        case 3:
                        case 4:
                            return LEFT;
                    }
            }
        } else {

        }
    }

    getLocationScore = () => {
        return (1000 * (this.currentNode.x + this.currentNode.xOffset + 1)) + (4 * (this.currentNode.y + this.currentNode.yOffset + 1)) + this.currentDirection;
    }

    createNodes = (USE_TEST_DATA, cube = false) => {
        this.nodes = [];
        this.cubeGrid = cube;
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
                        // Connect edge nodes only if using a flat map
                        if (!this.cubeGrid) this.connectEdges(node, faceLength, USE_TEST_DATA);
                    }
                })
            })
        })
        // If we folded this into a cube, determine edges
        if (this.cubeGrid) this.zipCube(faceLength, USE_TEST_DATA);
    }
    
    zipCube = (faceLength, USE_TEST_DATA) => {
        const first = 0;
        const last = faceLength - 1;

        for (let i = 0; i < faceLength; i++){
            const increasing = i;
            const decreasing = last - i;

            // Face 0 
            let currentFace = 0;
            let faceRelations = this.getFaceRelations(currentFace, USE_TEST_DATA)
            // Top
            this.nodes[currentFace][first][increasing].up = this.nodes[faceRelations.up][first][decreasing];
            this.connectOtherTopSides(this.nodes[currentFace][first][increasing], faceLength, increasing);
            // Bottom
            this.nodes[currentFace][last][increasing].down = this.nodes[faceRelations.down][first][increasing];
            this.connectOtherBottomSides(this.nodes[currentFace][last][increasing], faceLength, increasing);
            // Left
            this.nodes[currentFace][increasing][first].left = this.nodes[faceRelations.left][first][increasing];
            this.connectOtherLeftSides(this.nodes[currentFace][increasing][first], faceLength, increasing);
            // Right
            this.nodes[currentFace][increasing][last].right = this.nodes[faceRelations.right][decreasing][last];
            this.connectOtherRightSides(this.nodes[currentFace][increasing][last], faceLength, increasing);

            // Face 1
            currentFace = 1;
            faceRelations = this.getFaceRelations(currentFace, USE_TEST_DATA);
            // Top
            this.nodes[currentFace][first][increasing].up = this.nodes[faceRelations.up][first][decreasing];
            this.connectOtherTopSides(this.nodes[currentFace][first][increasing], faceLength, increasing);
            // Bottom
            this.nodes[currentFace][last][increasing].down = this.nodes[faceRelations.down][first][increasing];
            this.connectOtherBottomSides(this.nodes[currentFace][last][increasing], faceLength, increasing);
            // Left
            this.nodes[currentFace][increasing][first].left = this.nodes[faceRelations.left][last][decreasing];
            this.connectOtherLeftSides(this.nodes[currentFace][increasing][first], faceLength, increasing);
            // Right
            this.nodes[currentFace][increasing][last].right = this.nodes[faceRelations.right][increasing][first];
            this.connectOtherRightSides(this.nodes[currentFace][increasing][last], faceLength, increasing);

            // Face 2
            currentFace = 2;
            faceRelations = this.getFaceRelations(currentFace, USE_TEST_DATA);
            // Top
            this.nodes[currentFace][first][increasing].up = this.nodes[faceRelations.up][increasing][first];
            this.connectOtherTopSides(this.nodes[currentFace][first][increasing], faceLength, increasing);
            // Bottom
            this.nodes[currentFace][last][increasing].down = this.nodes[faceRelations.down][decreasing][first];
            this.connectOtherBottomSides(this.nodes[currentFace][last][increasing], faceLength, increasing);
            // Left
            this.nodes[currentFace][increasing][first].left = this.nodes[faceRelations.left][increasing][last];
            this.connectOtherLeftSides(this.nodes[currentFace][increasing][first], faceLength, increasing);
            // Right
            this.nodes[currentFace][increasing][last].right = this.nodes[faceRelations.right][increasing][first];
            this.connectOtherRightSides(this.nodes[currentFace][increasing][last], faceLength, increasing);

            // Face 3
            currentFace = 3;
            faceRelations = this.getFaceRelations(currentFace, USE_TEST_DATA);
            // Top
            this.nodes[currentFace][first][increasing].up = this.nodes[faceRelations.up][last][increasing];
            this.connectOtherTopSides(this.nodes[currentFace][first][increasing], faceLength, increasing);
            // Bottom
            this.nodes[currentFace][last][increasing].down = this.nodes[faceRelations.down][first][increasing];
            this.connectOtherBottomSides(this.nodes[currentFace][last][increasing], faceLength, increasing);
            // Left
            this.nodes[currentFace][increasing][first].left = this.nodes[faceRelations.left][increasing][last];
            this.connectOtherLeftSides(this.nodes[currentFace][increasing][first], faceLength, increasing);
            // Right
            this.nodes[currentFace][increasing][last].right = this.nodes[faceRelations.right][first][decreasing];
            this.connectOtherRightSides(this.nodes[currentFace][increasing][last], faceLength, increasing);

            // Face 4
            currentFace = 4;
            faceRelations = this.getFaceRelations(currentFace, USE_TEST_DATA);
            // Top
            this.nodes[currentFace][first][increasing].up = this.nodes[faceRelations.up][last][increasing];
            this.connectOtherTopSides(this.nodes[currentFace][first][increasing], faceLength, increasing);
            // Bottom
            this.nodes[currentFace][last][increasing].down = this.nodes[faceRelations.down][last][decreasing];
            this.connectOtherBottomSides(this.nodes[currentFace][last][increasing], faceLength, increasing);
            // Left
            this.nodes[currentFace][increasing][first].left = this.nodes[faceRelations.left][last][decreasing];
            this.connectOtherLeftSides(this.nodes[currentFace][increasing][first], faceLength, increasing);
            // Right
            this.nodes[currentFace][increasing][last].right = this.nodes[faceRelations.right][increasing][first];
            this.connectOtherRightSides(this.nodes[currentFace][increasing][last], faceLength, increasing);

            // Face 5
            currentFace = 5;
            faceRelations = this.getFaceRelations(currentFace, USE_TEST_DATA);
            // Top
            this.nodes[currentFace][first][increasing].up = this.nodes[faceRelations.up][decreasing][last];
            this.connectOtherTopSides(this.nodes[currentFace][first][increasing], faceLength, increasing);
            // Bottom
            this.nodes[currentFace][last][increasing].down = this.nodes[faceRelations.down][decreasing][first];
            this.connectOtherBottomSides(this.nodes[currentFace][last][increasing], faceLength, increasing);
            // Left
            this.nodes[currentFace][increasing][first].left = this.nodes[faceRelations.left][increasing][last];
            this.connectOtherLeftSides(this.nodes[currentFace][increasing][first], faceLength, increasing);
            // Right
            this.nodes[currentFace][increasing][last].right = this.nodes[faceRelations.right][decreasing][last];
            this.connectOtherRightSides(this.nodes[currentFace][increasing][last], faceLength, increasing);
        }
    }

    connectOtherTopSides = (node, faceLength, y) => {
        node.down = this.nodes[node.face][1][y]; // down always safe
        // Only add lefts and rights if they aren't going over the edge
        if (y != 0) node.left = this.nodes[node.face][0][y - 1];
        if (y != faceLength - 1) node.right = this.nodes[node.face][0][y + 1];
    }

    connectOtherBottomSides = (node, faceLength, y) => {
        node.up = this.nodes[node.face][faceLength - 2][y]; // up always safe
        if (y != 0) node.left = this.nodes[node.face][faceLength - 1][y - 1];
        if (y != faceLength -1) node.right = this.nodes[node.face][faceLength - 1][y + 1];
    }

    connectOtherLeftSides = (node, faceLength, x) => {
        node.right = this.nodes[node.face][x][1];// right always safe
        if (x != 0) node.up = this.nodes[node.face][x - 1][0];
        if (x != faceLength - 1) node.down = this.nodes[node.face][x + 1][0];
    }

    connectOtherRightSides = (node, faceLength, x) => {
        node.left = this.nodes[node.face][x][faceLength - 2];
        if (x != 0) node.up = this.nodes[node.face][x - 1][faceLength - 1];
        if (x != faceLength - 1) node.down = this.nodes[node.face][x + 1][faceLength - 1];
    }

    connectEdges = (node, faceLength, USE_TEST_DATA) => {
        let faceRelations = this.getFaceRelations(node.face, USE_TEST_DATA);
        if (this.cubeGrid){
            node.up = node.x == 0 ? this.nodes[faceRelations.up][faceLength - 1][node.y] : this.nodes[node.face][node.x - 1][node.y];
            node.down = node.x == faceLength - 1 ? this.nodes[faceRelations.down][0][node.y] : this.nodes[node.face][node.x + 1][node.y];
            node.left = node.y == 0 ? this.nodes[faceRelations.left][node.x][faceLength - 1] : this.nodes[node.face][node.x][node.y - 1];
            node.right = node.y == faceLength - 1 ? this.nodes[faceRelations.right][node.x][0] : this.nodes[node.face][node.x][node.y + 1];
        } else {
            node.up = node.x == 0 ? this.nodes[faceRelations.up][faceLength - 1][node.y] : this.nodes[node.face][node.x - 1][node.y];
            node.down = node.x == faceLength - 1 ? this.nodes[faceRelations.down][0][node.y] : this.nodes[node.face][node.x + 1][node.y];
            node.left = node.y == 0 ? this.nodes[faceRelations.left][node.x][faceLength - 1] : this.nodes[node.face][node.x][node.y - 1];
            node.right = node.y == faceLength - 1 ? this.nodes[faceRelations.right][node.x][0] : this.nodes[node.face][node.x][node.y + 1];
        }
    }

    getFaceRelations = (face, USE_TEST_DATA) => {
        if (USE_TEST_DATA){
            if (this.cubeGrid){
                switch (face) {
                    case 0:
                        return {up: 1, down: 3, left: 2, right: 5};
                    case 1:
                        return {up: 0, down: 4, left: 5, right: 2};
                    case 2:
                        return {up: 0, down: 4, left: 1, right: 3};
                    case 3:
                        return {up: 0, down: 4, left: 2, right: 5};
                    case 4:
                        return {up: 3, down: 1, left: 2, right: 5};
                    case 5:
                        return {up: 3, down: 1, left: 4, right: 0};
                }
            } else {
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
            }
        } else {
            if (this.cubeGrid){
                switch (face) {
                    case 0:
                        return {up: 5, down: 2, left: 3, right: 1};
                    case 1:
                        return {up: 5, down: 2, left: 0, right: 4};
                    case 2:
                        return {up: 0, down: 4, left: 3, right: 1};
                    case 3:
                        return {up: 2, down: 5, left: 0, right: 4};
                    case 4:
                        return {up: 2, down: 0, left: 3, right: 1};
                    case 5:
                        return {up: 3, down: 1, left: 0, right: 4};
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