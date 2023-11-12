const LIT = '#';
const UNLIT = '.';
const NUM_OF_PIXELS = 240;
const SCREEN_ROW_LENGTH = 40;

export default class CathodeRayTube{
    instructions;   // List of instructions for module to execute
    register;       // CPU register, starting value 1
    cycles;         // CPU cycles, increments based on instruction received
    startLogOnCycle; // First cycle to log signal strength
    logInterval;    // Interval on which to log subsequent signal strengths
    signalStrengths; // List of logged signal strengths
    screen;         // List of pixels, either LIT or UNLIT

    constructor(startLogOnCycle, logInterval){
        this.instructions = [];
        this.register = 1;
        this.cycles = 0;
        this.startLogOnCycle = startLogOnCycle;
        this.logInterval = logInterval;
        this.signalStrengths = [];
        this.screen = new Array(NUM_OF_PIXELS).fill(UNLIT);
    }

    // Determines if a signal strength value should be added to list
    logCycle = () => {
        if ((this.cycles - this.startLogOnCycle) % this.logInterval == 0){
            this.signalStrengths.push(this.register * this.cycles);
        }
    }

    // Executes instructions, drawing pixels and determining when to update register
    executeInstructions = () => {
        this.instructions.forEach(instruction => {
            if (instruction.match('noop')){
                this.drawPixel(this.cycles);
                this.cycles++;
                this.logCycle();
            } else if (instruction.match('addx')){
                for (let i = 0; i < 2; i++){
                    this.drawPixel(this.cycles);
                    this.cycles++;
                    this.logCycle();
                }
                this.register += parseInt(instruction.split(' ')[1]);
            }
        })
    }

    // Calculates sum of signal strengths
    getSignalStrengthSum = () => {
        let sum = 0;
        this.signalStrengths.forEach(signal => {
            sum += signal;
        })
        return sum;
    }

    // Adds pixel to screen list, depending on current cycle and register
    drawPixel = (position) => {
        // Update offset to draw subsequent rows
        const offset = Math.floor(position / SCREEN_ROW_LENGTH) * SCREEN_ROW_LENGTH;

        // Determine valid locations
        const drawingLocations = new Set();
        const spritePositions = [this.register, this.register -1, this.register +1];
        spritePositions.forEach(pixel => { if (pixel >= 0) drawingLocations.add(pixel + offset) });

        // Light pixel if needed
        if (drawingLocations.has(position)){
            this.screen[position] = LIT;
        }
    }

    // Returns a printable string of the screen
    drawScreen = () => {
        let pixelCount = 0;
        let display = '';
        this.screen.forEach(pixel => {
            pixelCount++;
            display += pixel;
            if (pixelCount % SCREEN_ROW_LENGTH == 0)
                display += '\n';
        })
        return display;
    }
}