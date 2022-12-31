class Node{
    face;
    x;
    y;
    rock;
    up;
    down;
    left;
    right;

    constructor(face, x, y, rock){
        this.face = face;
        this.x = x;
        this.y = y;
        this.rock = rock;
    }
}

export default class DiceMap{
    input;
    instructions;
    nodes;
    cubeGrid;

    constructor(){
        this.input = [];
        this.instructions = [];
        this.nodes = [];
    }

    createNodes = (cube = false) => {
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
        // For each line in face 1

        // For each line in faces 2, 3, 4

        // For each line in faces 5, 6

        // Connect nodes according to flat or cube shape
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