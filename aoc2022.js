import ElfCalories from "./01/ElfCalories.js";
import RockPaperScissors from "./02/RockPaperScissors.js";
import Rucksack from "./03/Rucksack.js";
import ElfAssignments from "./04/ElfAssignments.js";
import CrateStacking from "./05/CrateStacking.js";
import SignalCode from "./06/SignalCode.js";
import FileStructure from "./07/FileStructure.js";
import TreeHouse from "./08/TreeHouse.js";
import RopeBridge from "./09/RopeBridge.js";
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
    console.log(signal.getSignalIndex(14)); // Part 2
}

// Day 07
const day07 = async () => {
    const files = new FileStructure();
    files.commands = await DataManager.loadDataToList('./07/commands.txt');
    files.processCommands();
    console.log(files.sumDirsUnder(100000));

    // Part 2
    const diskSize = 70000000;
    const neededSpace = 30000000;
    const usedSpace = files.root.calculateSize();
    const unusedSpace = diskSize - usedSpace;
    const amountToDelete = neededSpace - unusedSpace;
    console.log(files.findSmallestDirOver(amountToDelete).size);
}

// Day 08
const day08 = async () => {
    const treehouse = new TreeHouse();
    treehouse.trees = await DataManager.loadDataToList('./08/trees.txt');
    treehouse.splitData();
    console.log(treehouse.countVisibleTrees());

    // Part 2
    console.log(treehouse.largestScenicScore);
}

// Day 09
const day09 = async () => {
    const rope = new RopeBridge(0);
    rope.instructions = await DataManager.loadDataToList('./09/instructions.txt');
    rope.giveInstructions();
    console.log(rope.tailLocations.size);

    // Part 2
    const bigRope = new RopeBridge(8);
    bigRope.instructions = await DataManager.loadDataToList('./09/instructions.txt');
    bigRope.giveInstructions();
    console.log(bigRope.tailLocations.size);
}

day09();