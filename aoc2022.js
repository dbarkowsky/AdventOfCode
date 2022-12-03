import ElfCalories from "./01/ElfCalories.js";
import RockPaperScissors from "./02/RockPaperScissors.js";
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

day02();
