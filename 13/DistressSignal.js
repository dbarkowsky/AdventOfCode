import stringArray from 'string-array';

export default class DistressSignal {
    data;
    orderingList;

    constructor(){
        this.data = [];
        this.orderingList = [];
    }

    // Break data into 2D list
    cleanData = () => {
        let tempData = [];
        for (let i = 0; i < this.data.length; i += 3){
            tempData.push([this.data[i], this.data[i + 1]]);
        }
        tempData = tempData.map(pair => {
            return [this.convertStringToList(pair[0]), this.convertStringToList(pair[1])];
        })
        tempData = this.convertStringsToInts(tempData);
        this.data = tempData;
    }

    // Convert arrays of strings into arrays of ints
    convertStringsToInts = (array) => {
        array = array.map(el => {
            if (Array.isArray(el)){
                return this.convertStringsToInts(el);
            } else {
                return parseInt(el);
            }
        })
        return array;
    } 

    // Convert a string with an array inside to the actual array
    // Planned a method, but found this module to use instead! (this is why I wanted JS)
    convertStringToList = (string) => {
        return stringArray.parse(string).array;
    }

    // Planning for data parse.... trim outer [] to start
    // iterate through string, push int, skip commas (need to account for nums > 9)
    // If single [ detected, continue counting ['s until they equal ]'s
    // Get everything between those two indexes, including the [] and repeat (recurse)
    // Return list

    // Replaced my original algorithm with this much simpler option. 
    // This solution also didn't affect the array variable, making it much more reliable
    // Compares left and right values, accounting for blanks, arrays, and ints
    compare = (left, right) => {
        // Check for empty arrays
        if (!left && left !== 0) return 1;
        if (!right && right !== 0) return -1;
    
        // If either is an array, make the other an array too and compare
        if (Array.isArray(left) || Array.isArray(right)) {
            const leftArr = Array.isArray(left) ? left : [left];
            const rightArr = Array.isArray(right) ? right : [right];
            return this.compareArrays(leftArr, rightArr);
        }
    
        // Return comparison result
        if (left === right) return 0;
        return left < right ? 1 : -1;
    }
    
    // Compares two arrays
    compareArrays = (left, right) => {
        // Loop through array, comparing values
        const maxLen = Math.max(left.length, right.length);
        for (let i = 0; i < maxLen; i++) {
            const order = this.compare(left[i], right[i]);
            // If not the same, return result, otherwise continue
            if (order !== 0) return order;
        }
    
        return 0;
    }

    // Loops through pairs of packets, counting correctly ordered ones
    countRightPairs = () => {
        let indexSum = 0;
        for (let i = 0; i < this.data.length; i++){
            if (this.compare(this.data[i][0], this.data[i][1]) === 1){
                indexSum += i + 1;
            }
        }
        return indexSum;
    }

    // Populates a list with each packet, now unpaired
    // Assumes data is already split and parsed
    populateOrderingList = () => {
        this.data.forEach(pair => {
            this.orderingList.push(pair[0]);
            this.orderingList.push(pair[1]);
        })
    }

    // Sorts packets in orderingList and finds product of indexes of two divider packets
    // Assumes orderingList has been populated
    sortPackets = () => {
        // Add two mandatory packets
        this.orderingList.push([[2]]);
        this.orderingList.push([[6]]);

        // Sort using previous method
        this.orderingList = this.orderingList.sort((packet1, packet2) => this.compare(packet2, packet1));

        // Find indexes of divider packets
        let indexOf2;
        let indexOf6;

        this.orderingList.forEach((packet, index) => {
            if (JSON.stringify(packet) == '[[2]]')
                indexOf2 = index + 1;
            if (JSON.stringify(packet) == '[[6]]')
                indexOf6 = index + 1;
        })
        
        // Return product
        return indexOf2 * indexOf6;
    }
}