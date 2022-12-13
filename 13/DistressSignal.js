export default class DistressSignal {
    data;

    constructor(){
        this.data = [];
    }

    // Break data into 2D list
    cleanData = () => {
        const tempData = [];
        for (let i = 0; i < this.data.length; i += 3){
            tempData.push([this.data[i], this.data[i + 1]]);
        }
        tempData.forEach(pair => {
            pair = [this.convertList(pair[0]), this.convertList(pair[1])];
        })
        this.data = tempData;
    }

    // Better idea.... trim outer [] to start
    // iterate through string, push int, skip commas
    // If single [ detected, continue counting ['s until they equal ]'s
    // Get everything between those two indexes, including the [] and repeat (recurse)
    // Return list

    // // Convert from string to actual list
    // convertList = (string) => {
    //     let newList = [];
    //     // If string is still encapsulated by [], trim them off
    //     if (string.at(0) == '['){
    //         string = string.substring(1, string.length -1);
    //         newList = string.split(',[').map(el => {
    //             if (isNaN(parseInt(el))){
    //                 return this.convertList(el);
    //             } else {
    //                 return parseInt(el);
    //             }
    //         });
    //     }
    //     return newList;
    // }

    compareLists = (left, right) => {
        let returnValue;
        // If both lists have values remaining
        while (left.length > 0 && right.length > 0){
            // If both values are ints
            if (typeof(left) == 'number' && typeof(right) == 'number'){
                if (left < right)
                    returnValue = true;
                else if (left > right)
                    returnValue = false;
                else
                    continue;
            }
            // If both values are lists
            else if (Array.isArray(left) && Array.isArray(right)){
                const lValue = left.shift();
                const rValue = right.shift();
                returnValue = this.compareLists(lValue, rValue);
            }
            // If only left is a list
            else if (typeof(left) == 'object' && typeof(right) == 'number'){
                returnValue = this.compareLists(lValue, [rValue]);
            }
            // If only right is a list
            else if (typeof(left) == 'number' && typeof(right) == 'object'){
                returnValue = this.compareLists([lValue], rValue);
            }

            if (returnValue === 'equal'){
                continue;
            } else return returnValue;
        }

        // If only left list has values remaining
        if (left.length > 0 && right.length == 0)
            returnValue = false;

        // If only right list has values remaining
        if (left.length == 0 && right.length > 0)
            returnValue = true;

        // If entire list was equal
        if (left.length == 0 && right.length == 0)
            return 'equal';
    }

    countRightPairs = () => {
        let count = 0;
        this.data.forEach(pair => {
            if (this.compareLists(pair[0], pair[1])){
                count++;
            }
        })
        return count;
    }
}