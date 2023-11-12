export default class CrateStacking{
    crateStacks;
    instructions;
    
    constructor(){
        this.crateStacks = [];
        this.instructions = [];
    }

    // Splits string into list of Crates
    splitStacks = () => {
        this.crateStacks = this.crateStacks.map(stack => {
            return stack.split('');
        })
    }

    // Converts string instructions to object for easy use
    analyzeInstruction = (instruction) => {
        const instructionList = instruction.split(' ');
        return {
            move: instructionList[1],
            from: instructionList[3],
            to: instructionList[5]
        }
    }

    // Carries out instructions, but only moves one at a time. (Order is reversed).
    performInstructionsCrane3000 = () => {
        this.instructions.forEach(instruction => {
            const instructionObj = this.analyzeInstruction(instruction);
            // For number of crates to move, pop from one stack, push on other stack
            for (let i = 0; i < instructionObj.move; i++){
                const crate = this.crateStacks[instructionObj.from - 1].pop();
                this.crateStacks[instructionObj.to - 1].push(crate);
            }
        });
    }

    // Carries out instructions, but keeps crates in order when moving them. (Picks up multiple at a time)
    performInstructionsCrane3001 = () => {
        this.instructions.forEach(instruction => {
            const instructionObj = this.analyzeInstruction(instruction);
            // Get crates to be moved...
            const movedCrates = this.crateStacks[instructionObj.from -1].slice(-instructionObj.move);
            // For each of those crates, pop a crate from one stack (and ditch it), push different crate onto other stack
            movedCrates.forEach(crate => {
                this.crateStacks[instructionObj.from -1].pop();
                this.crateStacks[instructionObj.to -1].push(crate);
            });
        });
    }

    // Gets the top crate in each stack and returns them as a string
    getTopCrates = () => {
        return this.crateStacks.map(stack => stack.at(-1)).join('');
    }
}