export default class MonkeyMath{
    monkeyList;
    monkeyObj;
    rawInput;

    constructor(){
        this.monkeyList = [];
        this.monkeyObj = {};
        this.rawInput = [];
    }

    // Puts monkeys from input into a list of objects
    assembleMonkeys = () => {
        this.rawInput.forEach(line => {
            let [name, value] = line.split(': ');
            if (parseInt(value)){
                this.monkeyList.push({
                    name,
                    value: parseInt(value)
                })
            } else {
                let [monkey1, operator, monkey2] = value.split(' ');
                this.monkeyList.push({
                    name,
                    value: undefined,
                    monkey1,
                    operator,
                    monkey2
                })
            }
        });
    }

    // Goes through monkeyList and adds to monkeyObj.
    // If part 2, returns a comparison for the root value of its two monkeys
    solveMonkeys = (isPart2 = false, humnValue = 0) => {
        this.monkeyObj = {};
        // Add all monkeys with known values to object
        this.monkeyList.forEach(monkey => {
            if (monkey.value){
                if (isPart2 && monkey.name == 'humn')
                    this.monkeyObj[monkey.name] = humnValue;
                else
                    this.monkeyObj[monkey.name] = monkey.value;
            }
        })

        // Take all operation monkeys. Go through queue, assessing them. If not yet possible, add back to queue.
        const monkeyQueue = [...this.monkeyList].filter(el => el.value == undefined); 
        while (monkeyQueue.length > 0){
            let currentMonkey = monkeyQueue.shift();
            // If both monkeys are assessed
            if (typeof this.monkeyObj[currentMonkey.monkey1] == 'number' && typeof this.monkeyObj[currentMonkey.monkey2] == 'number'){
                // Part 2, trying to see if both monkeys for root are equal, bigger, smaller...
                if (isPart2 && currentMonkey.name == 'root'){
                    if (this.monkeyObj[currentMonkey.monkey1] == this.monkeyObj[currentMonkey.monkey2])
                        return 0; // equal
                    else if (this.monkeyObj[currentMonkey.monkey1] > this.monkeyObj[currentMonkey.monkey2])
                        return 1; // monkey 1 bigger
                    else
                        return -1; // monkey 2 bigger
                }
                this.monkeyObj[currentMonkey.name] =  this.calculateMonkey(currentMonkey);
            } else {
                monkeyQueue.push(currentMonkey);
            }
        }
        console.log(this.monkeyObj);
    }

    // Do calculation of shouting monkey
    calculateMonkey = (monkey) => {
        switch (monkey.operator) {
            case '+':
                return this.monkeyObj[monkey.monkey1] + this.monkeyObj[monkey.monkey2];
            case '-':
                return this.monkeyObj[monkey.monkey1] - this.monkeyObj[monkey.monkey2];
            case '*':
                return this.monkeyObj[monkey.monkey1] * this.monkeyObj[monkey.monkey2];
            case '/':
                return this.monkeyObj[monkey.monkey1] / this.monkeyObj[monkey.monkey2];
        }
    }

    // Binary search to determine number
    determineHumanNumber = (min, max, humnOnLeft) => {
        let currMin = min;
        let currMax = max;

        while (currMin <= currMax){
            let currValue = Math.floor((currMin + currMax) / 2);
            if (currValue == 0) currValue++; // Try to avoid divide by 0, hope that 0 isn't answer
            //console.log(currMin, currMax, currValue)
            let result = this.solveMonkeys(true, currValue);

            // Which part does humn affect? left or right monkey of root?
            if (humnOnLeft){
                if (result == 0){
                    // Found!
                    return currValue;
                } else if (result == -1){
                    // Human number is too small
                    currMin = currValue;
                } else if (result == 1){
                    // Human number is too big
                    currMax = currValue;
                }
            } else {
                if (result == 0){
                    // Found!
                    return currValue;
                } else if (result == 1){
                    // Human number is too small
                    currMin = currValue;
                } else if (result == -1){
                    // Human number is too big
                    currMax = currValue;
                }
            }
        }
    }
}