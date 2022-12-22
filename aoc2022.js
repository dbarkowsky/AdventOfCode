import ElfCalories from "./01/ElfCalories.js";
import RockPaperScissors from "./02/RockPaperScissors.js";
import Rucksack from "./03/Rucksack.js";
import ElfAssignments from "./04/ElfAssignments.js";
import CrateStacking from "./05/CrateStacking.js";
import SignalCode from "./06/SignalCode.js";
import FileStructure from "./07/FileStructure.js";
import TreeHouse from "./08/TreeHouse.js";
import RopeBridge from "./09/RopeBridge.js";
import CathodeRayTube from "./10/CathodeRayTube.js";
import KeepAway from "./11/KeepAway.js";
import Heightmap from "./12/Heightmap.js";
import DistressSignal from "./13/DistressSignal.js";
import SandTrap from "./14/SandTrap.js";
import SensorSuite from "./15/SensorSuite.js";
import Valves from "./16/Valves.js";
import FallingRocks from "./17/FallingRocks.js";
import Obsidian from "./18/Obsidian.js";
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

    // Part 2
    scores.calculateTotalScore(true);
    console.log(scores.totalScore);
}

// Day 03
const day03 = async () => {
    const sack = new Rucksack();
    sack.contents = await DataManager.loadDataToList('./03/contents.txt');
    sack.compareCompartments();
    console.log(sack.sum);

    // Part 2
    sack.findBadges();
    console.log(sack.sum);
}

// Day 04
const day04 = async () => {
    const shifts = new ElfAssignments();
    shifts.shifts = await DataManager.loadDataToList('./04/pairs.txt');
    shifts.splitShifts();
    console.log(shifts.countOverlap(true)); // Full overlap

    // Part 2
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
    
    // Part 2
    console.log(signal.getSignalIndex(14)); 
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

// Day 10
const day10 = async () => {
    const tube = new CathodeRayTube(20, 40);
    tube.instructions = await DataManager.loadDataToList('./10/instructions.txt');
    tube.executeInstructions();
    console.log(tube.getSignalStrengthSum());
    
    // Part 2
    console.log(tube.drawScreen());
}

// Day 11
const day11 = async () => {
    const keepaway = new KeepAway();
    keepaway.monkeys = await DataManager.loadDataToList('./11/monkeys.txt');
    keepaway.convertMonkeys();
    keepaway.playKeepAway(20);
    console.log(keepaway.monkeys);
    console.log(keepaway.getMonkeyBusiness(2));

    // Part 2
    const keepawayPt2 = new KeepAway();
    keepawayPt2.monkeys = await DataManager.loadDataToList('./11/monkeys.txt');
    keepawayPt2.convertMonkeys();
    keepawayPt2.playKeepAway(10000, false);
    console.log(keepawayPt2.getMonkeyBusiness(2));
}

// Day 12
const day12 = async () => {
    const map = new Heightmap();
    map.map = await DataManager.loadDataToList('./12/map.txt');
    map.splitMapRows();
    //console.log(map.map);
    map.findStartingNode();
    console.log(map.findPathLength(map.startingNode));

    // Part 2
    console.log(map.findShortestAPath());
}

// Day 13
const day13 = async () => {
    const signal = new DistressSignal();
    signal.data = await DataManager.loadDataToList('./13/input.txt');
    signal.cleanData();
    console.log(signal.countRightPairs());

    // Part 2
    signal.populateOrderingList();
    console.log(signal.sortPackets());
}

// Day 14
const day14 = async () => {
    const trap = new SandTrap({y: 500, x: 0});
    trap.rockInstructions = await DataManager.loadDataToList('./14/input.txt');
    trap.parseInput();
    trap.drawRock();
    trap.springTrap();
    trap.printGrid();
    console.log(trap.sandCounter);

    // Part 2
    const trapWithFloor = new SandTrap({y: 500, x: 0}, false);
    trapWithFloor.rockInstructions = await DataManager.loadDataToList('./14/input.txt');
    trapWithFloor.parseInput();
    trapWithFloor.drawRock();
    trapWithFloor.springTrap();
    trapWithFloor.printGrid();
    console.log(trapWithFloor.sandCounter);
}

// Day 15
const day15 = async () => {
    const sensors = new SensorSuite();
    sensors.readings = await DataManager.loadDataToList('./15/sensor_readings.txt');
    sensors.parseReadings();
    console.log(sensors.countCoverageOnRow(2000000));

    // Part 2
    console.log(sensors.calculateFrequency(4000000));
}

// Day 16
const day16 = async () => {
    const valves = new Valves();
    valves.rawInput = await DataManager.loadDataToList('./16/valves.txt');
    valves.parseInput();
    console.log(valves.valves);
    console.log(valves.findGreatestPressure('AA', 30, 0, true));
}

// Day 17 
const day17 = async () => {
    const rockTetris = new FallingRocks(7, 20);
    rockTetris.jetPattern = await DataManager.loadData('./17/jetPattern.txt');
    rockTetris.parsePattern();
    rockTetris.dropManyRocks(2022);
    rockTetris.printGrid();
    console.log(rockTetris.highestX);

    // Part 2
    const rockTetrisPart2 = new FallingRocks(7, 20);
    rockTetrisPart2.jetPattern = await DataManager.loadData('./17/jetPattern.txt');
    rockTetrisPart2.parsePattern();
    //let rocksToDrop = parseFloat(rockTetrisPart2.jetPattern.length * rockTetrisPart2.rocks.length);
    rockTetrisPart2.dropManyRocks(1000000000000);
    //console.log(rockTetrisPart2.getHighestRockRow() * (1000000000000.0 / rocksToDrop));
    console.log(rockTetris.highestX);
}

// Day 18
const day18 = async () => {
    const obsidianCubes = new Obsidian();
    obsidianCubes.rawInput = await DataManager.loadDataToList('./18/scan.txt');
    obsidianCubes.buildCubes();
    console.log(obsidianCubes.cubes);
    console.log(obsidianCubes.countExposedSides());
}

day18();