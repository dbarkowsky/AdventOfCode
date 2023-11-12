export default class SignalCode{
    signal;

    constructor(){
        this.signal = '';
    }

    // Splits the signal into a list
    splitSignal = () => {
        this.signal = this.signal.split('');
    }

    // Finds the first instance of a unique sequence of blockLength long
    // Returns the last index of that sequence + 1
    getSignalIndex = (blockLength) => {
        const currentStartCandidate = [];
        let signalIndex = 0;
        while(
            !this.signalStartDetected(currentStartCandidate) || 
            currentStartCandidate.length < blockLength
        ){
            if (currentStartCandidate.length >= blockLength)
                currentStartCandidate.shift();
            currentStartCandidate.push(this.signal[signalIndex]);
            signalIndex++;
            
        }
        return signalIndex;
    }

    // Checks to see if all values in possibleStart list are unique
    // If all unique, Set's size should equal length of list
    signalStartDetected = (possibleStart) => {
        return new Set(possibleStart).size == possibleStart.length;
    }
}