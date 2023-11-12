export default class Balloons{
    snafu;
    decimal;
    
    constructor(){
        this.snafu = [];
        this.decimal = [];
    }

    // Converts snafu string into decimal number
    snafuToDecimal = (snafu) => {
        let base = 1;
        let sum = 0;
        const snafuList = snafu.split('');

        // For each symbol, determine value and sum
        while (snafuList.length > 0){
            const value = snafuList.pop();

            if (parseInt(value)){
                sum += parseInt(value) * base;
            } else {
                switch (value) {
                    case '=':
                        sum -= base * 2;
                        break;
                    case '-':
                        sum -= base;
                        break;
                }
            }
            base *= 5;
        }

        return sum;
    }

    // Converts decimal number into snafu string
    decimalToSnafu = (decimal) => {
        // Determine biggest square of 5 we need
        let biggestDivisor = 1;
        while (decimal / biggestDivisor >= 1){
                biggestDivisor *= 5;
        }
        biggestDivisor /= 5; // take it back a step
        let placesArr = []; // extra place for 1's position

        // Put values into list in reverse order, stop after 1s position
        let currentDivisor = biggestDivisor;
        let remainder = decimal;
        while(currentDivisor >= 1){
            placesArr.unshift(Math.floor(remainder / currentDivisor));
            remainder = remainder % currentDivisor;
            currentDivisor /= 5;
        }
        
        // Go through array. Start at top end, if value is above 2, must subtract 5 and add 1 to higher value
        let invalidValuesExist = true;
        while (invalidValuesExist){
            invalidValuesExist = false;
            for (let i = placesArr.length - 1; i >= 0; i--){
                if (placesArr[i] > 2){
                    invalidValuesExist = true;
                    placesArr[i] -= 5;
                    placesArr[i + 1]++;
                }
            }
        }

        // Convert values to snafu symbols
        placesArr = placesArr.map(el => {
            if (el == -2)
                return '=';
            if (el == -1)
                return '-';
            return `${el}`;
        })

        // Pop values off and concat to string
        let returnString = '';
        for (let i = placesArr.length - 1; i >= 0; i--){
            returnString += placesArr[i];
        }
        return returnString;
    }

    // Converts all snafu to decimal list
    convertSnafu = () => {
        this.snafu.forEach(num => {
            this.decimal.push(this.snafuToDecimal(num));
        })
    }

    // Gets the sum of the decimal list
    getDecimalSum = () => this.decimal.reduce((acc, val) => acc + val, 0);
    
}
