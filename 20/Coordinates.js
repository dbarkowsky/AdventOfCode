export default class Coordinates{
    values;

    constructor(){
        this.values = [];
    }

    convertValues = () => {
        this.values = this.values.map((value, index) => {
            return {
                value: parseInt(value),
                startingIndex: index,
                currentIndex: index
            }
        })
    }

    processSingleValue = (startingIndex) => {
        // Find matching node
        let currentIndex = this.values.findIndex(el => el.startingIndex == startingIndex);
        let currentNode = this.values.at(currentIndex);

        // Determine new index of node
        let newIndex = currentIndex;
        // console.log(newIndex, currentNode.value);
        // if going down
        if (currentNode.value < 0){
            for (let i = 0; i < Math.abs(currentNode.value); i++){
                newIndex--;
                if (newIndex < 0) newIndex = this.values.length - 1;
            }
            // What is the startingIndex of what's currently in that index?
            const existingNode = this.values.at(newIndex);

            // Remove original
            this.values.splice(currentIndex, 1);

            // Find node to insert before
            const existingIndex = this.values.findIndex(el => el.startingIndex == existingNode.startingIndex);

            // Insert new node
            this.values.splice(existingIndex, 0, currentNode);

            // If moved to the beginning, put it at the end instead
            if(newIndex == 0){
                this.values.push(this.values.shift());
            }
            
        // if going up
        } else if (currentNode.value > 0){
            newIndex = (newIndex + currentNode.value) % this.values.length;
            // What is the startingIndex of what's currently in that index?
            const existingNode = this.values.at(newIndex);

            // Remove original
            this.values.splice(currentIndex, 1);

            // Find node to insert after
            const existingIndex = this.values.findIndex(el => el.startingIndex == existingNode.startingIndex);

            // Insert new node
            this.values.splice(existingIndex + 1, 0, currentNode);
        }
    }

    processAllValues = () => {
        for (let i = 0; i < this.values.length; i++){
            this.processSingleValue(i);
        }
        this.updateCurrentIndexes();
    }

    sumCoordinates = (interval, max) => {
        let sum = 0;
        // Where is 0?
        const zeroIndex = this.values.findIndex(el => el.value == 0);
        let currentCount = 0;
        //console.log('zeroIndex', zeroIndex)
        for (let i = interval ; i <= max; i += interval){
            if (i % interval == 0){
                sum += this.values[(zeroIndex + i) % this.values.length].value;
                console.log(`value ${this.values[(zeroIndex + i) % this.values.length].value} at index ${(zeroIndex + i) % this.values.length}`)
            }
            currentCount++;
        }

        return sum;
    }

    updateCurrentIndexes = () => {
        for (let i = 0; i < this.values.length; i++){
            this.values[i].currentIndex = i;
        }
    }

    toString = () => this.values.map(el => '\n' + `i: ${el.currentIndex}, v: ${el.value}`).toString();
}