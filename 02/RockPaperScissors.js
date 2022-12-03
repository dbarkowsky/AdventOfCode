
export default class RockPaperScissors{
    data;
    totalScore;

    constructor(){
        this.data = [];
        this.totalScore = 0;
    }

    // Determines which hand to play, following part 2 rules
    determineYourHand = (opponentsHand, outcome) => {
        // A/X = rock, B/Y = paper, C/Z = scissors
        // X = lose, Y = draw, Z = win
        switch (opponentsHand) {
            case 'A': // rock
                switch (outcome) {
                    case 'X': // lose
                        return 'Z'; // scissors
                    case 'Y': // draw
                        return 'X'; // rock
                    case 'Z': // win
                        return 'Y'; // paper
                    default:
                        break;
                }
                break;
            case 'B': // paper
                switch (outcome) {
                    case 'X': // lose
                        return 'X'; // rock
                    case 'Y': // draw
                        return 'Y'; // paper
                    case 'Z': // win
                        return 'Z'; // scissors
                    default:
                        break;
                }
                break;
            case 'C': // scissors
                switch (outcome) {
                    case 'X': // lose
                        return 'Y'; // paper
                    case 'Y': // draw
                        return 'Z'; // scissors
                    case 'Z': // win
                        return 'X'; // rock
                    default:
                        break;
                }
                break;        
            default:
                break;
        }
    }
    
    // Calculates score of a single rock paper scissors round
    calculateScore = (opponentsHand, yourHand) => {
        // A/X is rock
        // B/Y is paper
        // C/Z is scissors
        let score = 0;
        switch (yourHand) {
            case 'X':
                score += 1;
                switch (opponentsHand) {
                    case 'A': // draw
                        score += 3;
                        break;
                    case 'C': // win
                        score += 6;
                        break;
                    default:
                        break;
                }
                break;
            case 'Y':
                score += 2;
                switch (opponentsHand) {
                    case 'A': // win
                        score += 6;
                        break;
                    case 'B': // draw
                        score += 3;
                        break;
                    default:
                        break;
                }
                break;
            case 'Z':
                score += 3;
                switch (opponentsHand) {
                    case 'B': // win
                        score += 6;
                        break;
                    case 'C': // draw
                        score += 3;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        return score;
    }

    // Calculates total score from all rounds
    // boolean parameter: false if second column is your hand, true if it's the desired outcome
    calculateTotalScore = (secondColumnIsOutcome = false) => {
        this.totalScore = 0;
        // Assumes data is 2D array at this point
        this.data.forEach(pair => {
            if (secondColumnIsOutcome){
                this.totalScore += this.calculateScore(pair[0], this.determineYourHand(pair[0], pair[1]));
            } else {
                this.totalScore += this.calculateScore(pair[0], pair[1]);
            }
        });
    }

    // Takes list of data rows and converts it to 2D array
    refineData = () => {
        this.data = this.data.map(pair => {
            return [pair.at(0), pair.at(2)]; // Expecting 'A X' format
        })
    }
}
