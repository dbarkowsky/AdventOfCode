import ElfCalories from "./01/ElfCalories.js";
import RockPaperScissors from "./02/RockPaperScissors.js";
import Rucksack from "./03/Rucksack.js";
import ElfAssignments from "./04/ElfAssignments.js";
import CrateStacking from "./05/CrateStacking.js";
import SignalCode from "./06/SignalCode.js";
import DataManager from "./common/DataManager.js";

// Day 01
const day01 = async () => {
    const calories = new ElfCalories();
    calories.data = await DataManager.loadDataToList('./01/calories.txt');
    calories.sumCalories();
    calories.sortCalories();
    console.log(calories.elfList);
    console.log(calories.elfList[0] + calories.elfList[1] + calories.elfList[2]);
}

// Day 02
const day02 = async () => {
    const scores = new RockPaperScissors();
    scores.data = await DataManager.loadDataToList('./02/rounds.txt');
    scores.refineData();
    scores.calculateTotalScore();
    console.log(scores.totalScore);
    scores.calculateTotalScore(true);
    console.log(scores.totalScore);
}

// Day 03
const day03 = async () => {
    const sack = new Rucksack();
    sack.contents = await DataManager.loadDataToList('./03/contents.txt');
    sack.compareCompartments();
    console.log(sack.sum);
    sack.findBadges();
    console.log(sack.sum);
}

// Day 04
const day04 = async () => {
    const shifts = new ElfAssignments();
    shifts.shifts = await DataManager.loadDataToList('./04/pairs.txt');
    shifts.splitShifts();
    console.log(shifts.countOverlap(true)); // Full overlap
    console.log(shifts.countOverlap()); // Partial overlap
}

// Day 05
const day05 = async () => {
    const crateStack = new CrateStacking();
    crateStack.crateStacks = await DataManager.loadDataToList('./05/startingPositions.txt');
    crateStack.instructions = await DataManager.loadDataToList('./05/instructions.txt');
    crateStack.splitStacks();
    crateStack.performInstructionsCrane3000();
    console.log(crateStack.getTopCrates());
    // Part 2
    crateStack.crateStacks = await DataManager.loadDataToList('./05/startingPositions.txt');
    crateStack.splitStacks();
    crateStack.performInstructionsCrane3001();
    console.log(crateStack.getTopCrates());
}

// Day 06
const day06 = async () => {
    const signal = new SignalCode();
    signal.signal = await DataManager.loadData('./06/signal.txt');
    signal.splitSignal();
    console.log(signal.signal);
    console.log(signal.getSignalIndex(4));
    console.log(signal.getSignalIndex(14));
}

day06();
