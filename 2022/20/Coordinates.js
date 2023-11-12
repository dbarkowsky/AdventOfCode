const DECRYPTION_KEY = 811589153;

export default class Coordinates{
    values;

    constructor(){
        this.values = [];
    }

    // Converts values to objects
    convertValues = () => {
        this.values = this.values.map((value, index) => {
            return {
                value: parseInt(value),
                startingIndex: index,
                currentIndex: index
            }
        })
    }

    // Looks at a single value and moves it left or right accordingly
    // Thanks to u/MrJohnnyS for helping me see how I could minimize this! Way faster
    processSingleValue = (startingIndex) => {
        // Find matching node
        let currentIndex = this.values.findIndex(el => el.startingIndex == startingIndex);
        let currentNode = this.values.at(currentIndex);

        // Remove node
        this.values.splice(currentIndex, 1); 
        // Place node at a location offset from the current location
        this.values.splice((currentNode.value + currentIndex) % this.values.length, 0, currentNode);
    }

    // Looks at all values in list. Makes call to move them.
    processAllValues = (times = 1) => {
        for (let time = 0; time < times; time++){
            for (let i = 0; i < this.values.length; i++){
                this.processSingleValue(i);
            }
        }
    }

    // Uses decryption key to alter values 
    decryptValues = () => {
        this.values.forEach(value => value.value *= DECRYPTION_KEY);
    }

    // Gets the sum at specified intervals
    sumCoordinates = (interval, max) => {
        let sum = 0;
        // Where is 0?
        const zeroIndex = this.values.findIndex(el => el.value == 0);
        for (let i = interval ; i <= max; i += interval){
            if (i % interval == 0){
                sum += this.values[(zeroIndex + i) % this.values.length].value;
                console.log(`value ${this.values[(zeroIndex + i) % this.values.length].value} at index ${(zeroIndex + i) % this.values.length}`)
            }
        }

        return sum;
    }

    // Updates indexes by running through values list. Never used.
    updateCurrentIndexes = () => {
        for (let i = 0; i < this.values.length; i++){
            this.values[i].currentIndex = i;
        }
    }

    // Prints value to string to help with tracking
    toString = () => this.values.map(el => '\n' + `i: ${el.currentIndex}, v: ${el.value}`).toString();
}