
export default class ElfCalories{
    elfList;
    data;

    constructor(){
        this.elfList = [];
        this.data;
    }

    // sums groups of calories and adds to list
    sumCalories = () => {
        let runningTotal = 0;
        this.data.forEach((line) => {
            if (line != '') {
                runningTotal += parseInt(line);
            } else {
                this.elfList.push(runningTotal);
                runningTotal = 0;  
            }
        })
    }
    
    // sorts biggest to smallest
    sortCalories = async () => {
        this.elfList.sort((a, b) => b - a);
    }
}