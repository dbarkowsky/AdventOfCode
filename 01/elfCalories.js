import fs from 'fs';
import readline from 'readline';

export default class ElfCalories{
    elfList;

    constructor(){
        this.dataStream = fs.createReadStream('./01/calories.txt', 'utf-8');
        this.elfList = [];
    }

    // for testing purposes
    printData = () => {
        this.dataStream.on('error', function (error) {
            console.log(`error: ${error.message}`);
        })
    
        this.dataStream.on('data', (chunk) => {
            console.log(chunk);
            console.log(typeof(chunk));
        })
    }

    // sums groups of calories and adds to list
    sumCalories = () => {
        const read = readline.createInterface(
            {
                input: this.dataStream,
                terminal: false
            }
        )
        let runningTotal = 0;
        read.on('line', (line) => {
            
            if (line != '') {
                runningTotal += parseInt(line);
            } else {
                this.elfList.push(runningTotal);
                runningTotal = 0;
                
            }
        })
    }

    // sorts biggest to smallest
    sortCalories = () => {
        this.elfList.sort((a, b) => b - a);
    }
}