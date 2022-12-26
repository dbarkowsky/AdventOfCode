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
        console.log(newIndex, currentNode.value);
        // if going down
        if (currentNode.value < 0){
            for (let i = 0; i < Math.abs(currentNode.value); i++){
                newIndex--;
                if (newIndex < 0) newIndex = this.values.length - 1;
            }
        // if going up
        } else if (currentNode.value > 0){
            newIndex = (newIndex + currentNode.value) % this.values.length;
        }

        if (currentNode.value != 0){
            // What is the startingIndex of what's currently in that index?
            const existingNode = this.values.at(newIndex);
            const existingStartingIndex = existingNode.startingIndex;

            // Will it move if we remove an element?
            if (currentNode.currentIndex < newIndex)
                newIndex--;

            // Remove original
            this.values.splice(currentIndex, 1);

            // Update current object
            currentNode.currentIndex = newIndex;

            // Insert new node
            this.values.splice(currentNode.currentIndex, 0, currentNode);
        }

        this.updateCurrentIndexes();
    }

    updateCurrentIndexes = () => {
        for (let i = 0; i < this.values.length; i++){
            this.values[i].currentIndex = i;
        }
    }

    toString = () => this.values.map(el => el.value).toString();
}