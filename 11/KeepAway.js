// A single Monkey.
class Monkey {
  items; // List of items that monkey has
  operationOperator; // Operator how monkey inflates item
  operationModifier; // Value by which item is inflated
  testDivisor; // Value by which item is reduced
  trueMonkey; // Pass to this monkey if true
  falseMonkey; // Pass to this monkey if false
  inspectCount; // How many times monkey inspects items.

  constructor(
    startingItems,
    operationOperator,
    operationModifier,
    testDivisor,
    trueMonkey,
    falseMonkey
  ) {
    this.items = startingItems;
    this.operationOperator = operationOperator;
    this.operationModifier = operationModifier;
    this.testDivisor = parseInt(testDivisor);
    this.trueMonkey = parseInt(trueMonkey);
    this.falseMonkey = parseInt(falseMonkey);
    this.inspectCount = 0;
  }

  // Monkey inspects item, inflating item
  inspect = (item) => {
    this.inspectCount++;
    let modifier;
    if (this.operationModifier == 'old'){
      modifier = item;
    } else {
      modifier = parseInt(this.operationModifier);
    }

    if (this.operationOperator == '+')
      return item + modifier;
    if (this.operationOperator == '*')
      return item * modifier;
  }

  // Monkey gets bored, reducing item
  getBored = (item, divisor = 3) => {
    return Math.floor(item / divisor);
  }

  // Monkey tests item against his values
  test = (item) => (item % this.testDivisor == 0);
}

export default class KeepAway {
  monkeys; // A list of monkeys
  round; // Count of which round we're on
  commonMultiple; // A common multiple of all items

  constructor() {
    this.monkeys = [];
    this.round = 1;
    this.commonMultiple = 1;
  }

  // Convert string data into usable monkeys
  convertMonkeys = () => {
    const tempMonkeys = [];
    for (let line = 0; line < this.monkeys.length; line += 7){ // 7 lines per monkey
      const tempItems = this.monkeys[line + 1].substring(this.monkeys[line + 1].indexOf(': ') + 2).split(', ').map(item => parseInt(item));
      const [tempOperator, tempModifier] = this.monkeys[line + 2].substring(this.monkeys[line + 2].indexOf('old ') + 4).split(' ');
      const tempTest = this.monkeys[line + 3].substring(this.monkeys[line + 3].indexOf('divisible by') + 13);
      const tempTrue = this.monkeys[line + 4].substring(this.monkeys[line + 4].indexOf('monkey') + 7);
      const tempFalse = this.monkeys[line + 5].substring(this.monkeys[line + 5].indexOf('monkey') + 7);
      let newMonkey = new Monkey(
        tempItems,
        tempOperator,
        tempModifier,
        tempTest,
        tempTrue,
        tempFalse
      );
      tempMonkeys.push(newMonkey);
    }
    this.monkeys = tempMonkeys;
  }

  // Calculates a common multiple of all items to start.
  calcCM = () => {
    this.commonMultiple = this.monkeys.reduce((a, b) => a * b.testDivisor, 1);
  }

  // Monkeys play the game.
  // For each round, for each monkey, for each item...
  playKeepAway = (rounds = 1, stillWorried = true) => {
    this.calcCM();
    for (let round = 0; round < rounds; round++){
      for (let currentMonkey = 0; currentMonkey < this.monkeys.length; currentMonkey++) {
        while(this.monkeys[currentMonkey].items.length > 0){
          // Get item
          let item = this.monkeys[currentMonkey].items.shift();
          // Inspect/inflate
          item = this.monkeys[currentMonkey].inspect(item);
          // Reduce
          if (stillWorried){
            item = this.monkeys[currentMonkey].getBored(item);
          } else {
            item = item % this.commonMultiple;
          }
          // Test and pass
          if (this.monkeys[currentMonkey].test(item)){
            this.monkeys[this.monkeys[currentMonkey].trueMonkey].items.push(item);
          } else {
            this.monkeys[this.monkeys[currentMonkey].falseMonkey].items.push(item);
          }
        }
      }
    }
  }

  // Calculates monkey business, product of top amount of monkeys 
  getMonkeyBusiness = (howManyMonkeys) => {
    const values = this.monkeys.map(monkey => monkey.inspectCount);
    values.sort((a, b) => b - a);
    let product = 1;
    for (let i = 0; i < howManyMonkeys; i++){
      product *= values[i];
    }
    return product;
  }
}
