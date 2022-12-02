import ElfCalories from "./01/elfCalories.js";

// Day 01
const calories = new ElfCalories();
calories.sumCalories();
// need to wait for list to populate... should go async in future
setTimeout(() => {
    calories.sortCalories();
    console.log(calories.elfList);
    console.log(calories.elfList[0] + calories.elfList[1] + calories.elfList[2]);
}, 5000);
