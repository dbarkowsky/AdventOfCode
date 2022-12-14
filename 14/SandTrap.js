const AIR = 0;
const ROCK = 1;
const SAND = 2;

class Node {
    contents;
    x;
    y;

    constructor(x, y, contents = AIR){
        this.contents = contents;
        this.x = x;
        this.y = y;
    }
}

export default class SandTrap{
    grid;
    rockInstructions;
    lowestY;
    highestY;
    highestX;
    
    constructor() {
        this.grid = [];
        this.rockInstructions = [];
        this.lowestY = Infinity;
        this.highestY = 0;
        this.highestX = 0;
    }

    parseInput = () => {
        this.rockInstructions = this.rockInstructions.map(line => {
            return line.split(' -> ').map(pair => {
                return pair.split(',').map(number => parseInt(number));
            });
        });

        this.findLowestPoints();
        this.assembleGrid();
    }

    findLowestPoints = () => {
        this.rockInstructions.forEach(line => {
            line.forEach(pair => {
                this.lowestY = this.lowestY > pair[0] ? pair[0] : this.lowestY;
                this.highestY = this.highestY > pair[0] ? this.highestY : pair[0];
                this.highestX = this.highestX > pair[1] ? this.highestX : pair[1];
            })
        })
    }

    assembleGrid = () => {
        const gridWidth = this.highestY - this.lowestY + 3; // 2 for space on left and right, 1 for inclusivity
        this.grid = new Array(this.highestX + 1).fill(new Array(gridWidth));
        
        for (let x = 0; x < this.grid.length; x++){
            for (let y = 0; y < this.grid[x].length; y++){
                this.grid[x][y] = new Node(x, y);
            }
        }
    }

    printGrid = () => {
        let printString = '';
        this.grid.forEach(line => {
            line.forEach(node => {
                switch (node.contents){
                    case AIR:
                        printString += '.';
                        break;
                    case ROCK:
                        printString += '#';
                        break;
                    case SAND:
                        printString += 'O';
                        break;
                }
            })
            printString += '\n';
        })
        printString += '\n';
        console.log(printString);
    }

    drawRock = () => {

    }
}