
export default class Rucksack{
    contents;
    separatedContents;
    sum;
    letterList;
    
    constructor(){
        this.contents = [];
        this.separatedContents = [];
        this.sum = 0;
        this.letterList = this.buildLetterList();
    }

    // Constructs alphabet list
    buildLetterList = () => {
        const list = [];
        const LETTERS_IN_ALPHA = 26;
        const LOWERCASE_OFFSET = 97;
        const UPPERCASE_OFFSET = 65;
        for (let i = 0; i < LETTERS_IN_ALPHA; i++){
            list.push(String.fromCharCode(i + LOWERCASE_OFFSET));
        }
        for (let i = 0; i < LETTERS_IN_ALPHA; i++){
            list.push(String.fromCharCode(i + UPPERCASE_OFFSET));
        }
        return list;
    }

    // Splits single rucksack string into two compartments
    splitContents = () => {
        this.separatedContents = this.contents.map(rucksack => {
            const list = [];
            list.push(rucksack.substring(0, rucksack.length/2)); // First half
            list.push(rucksack.substring(rucksack.length/2)); // Second half
            return list;
        })
    }

    // Compares two compartments in a rucksack.
    // If there is a matching character between both sides, add it's index +1 from alpha list to sum
    compareCompartments = () => {
        this.splitContents();
        this.sum = 0; // reset sum
        this.separatedContents.forEach(rucksack => {
            const compartment1 = rucksack[0];
            const compartment2 = rucksack[1];
            // for each letter in compartment1
            let alreadyFound = false;
            compartment1.split('').forEach(letter => {
                if (compartment2.includes(letter) && !alreadyFound){
                    alreadyFound = true; // to stop same letter from being added multiple times
                    this.sum += this.getCharValue(letter);
                }
            });
        })
    }

    // Examines 3 contents at a time, checks for matching letter, sums values
    findBadges = () => {
        this.sum = 0; // reset sum
        // skip three at a time
        for (let i = 0; i < this.contents.length; i += 3){
            const sack1 = this.contents[i];
            const sack2 = this.contents[i+1];
            const sack3 = this.contents[i+2];

            let alreadyFound = false;
            sack1.split('').forEach(letter => {
                if (
                    sack2.includes(letter) &&
                    sack3.includes(letter) &&
                    !alreadyFound
                ){
                    alreadyFound = true;
                    this.sum += this.getCharValue(letter);
                }
            });
        }
    }

    // Returns the numeric index value from the alphabet list
    // + 1 because scoring starts at 1, index starts at 0
    getCharValue = (letter) => this.letterList.indexOf(letter) + 1;
}
