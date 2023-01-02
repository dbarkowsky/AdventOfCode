export default class Balloons{
    snafu;
    decimal;
    
    constructor(){
        this.snafu = [];
        this.decimal = [];
    }

    snafuToDecimal = (snafu) => {
        let base = 1;
        let sum = 0;
        const snafuList = snafu.split('');

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

    decimalToSnafu = (decimal) => {
        // Determine biggest square of 5 we need
        let biggestDivisor = 1;
        while (decimal / biggestDivisor >= 1){
                biggestDivisor *= 5;
        }
        biggestDivisor /= 5; // take it back a step
        let placesArr = new Array(Math.floor(Math.pow(biggestDivisor, 1/5)) + 1); // extra place for 1's position

        // 5th root of place determines index and vice versa
        // Put values into 
        let currentDivisor = biggestDivisor;
        let remainder = decimal;
        for (let index = Math.floor(Math.pow(biggestDivisor, 1/5)); index >= 0; index--){
            placesArr[index] = Math.floor(remainder / currentDivisor);
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

    convertSnafu = () => {
        this.snafu.forEach(num => {
            this.decimal.push(this.snafuToDecimal(num));
        })
    }

    getDecimalSum = () => this.decimal.reduce((acc, val) => acc + val, 0);
    
}
// 4890 / 3125 = 1
// 4890 % 3125 = 1765
// 1765 / 625 = 2
// 1765 % 625 = 515
// 515 / 125 = 4 ~~~ not allowed so how to fix?  -> must subtract 5, then add 1 to higher value so 4 -> '-', 2 -> 3, then repeat
// 515 % 125 = 15
// 15 / 25 = 0
// 15 % 25 = 15
// 15 / 5 = 3 ~~~ not allowed
// 15 % 5 = 0

// should be 2=-1=0   3125,625,125,25,5,1
// +3125 * 2
// -625 * 2
// -125 * 1
// +25 * 1
// -5 * 2
// 1 * 0