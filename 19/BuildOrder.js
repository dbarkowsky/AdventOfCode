const ORE = 0;
const CLAY = 1;
const OBSIDIAN = 2;
const GEODE = 3;

// Represents a single snapshot in time along a specific path...
class StepNode{
    resources;
    robots;
    minute;
    blueprint;
    score;

    constructor(blueprint, robots, resources){
        this.robots = robots;
        this.resources = resources;
        this.minute = 0;
        this.blueprint = blueprint;
        this.score = 0;
    }

    collect = () => {
        this.minute++;
        this.resources.ore += this.robots.ore;
        this.resources.clay += this.robots.clay;
        this.resources.obsidian += this.robots.obsidian;
        this.resources.geode += this.robots.geode;
        return this;
    }

    buyOreBot = () => {
        this.robots.ore++;
        this.resources.ore -= this.blueprint.oreRobotOreCost;
        return this;
    }

    buyClayBot = () => {
        this.robots.clay++;
        this.resources.ore -= this.blueprint.clayRobotOreCost;
        return this;
    }

    buyObsidianBot = () => {
        this.robots.obsidian++;
        this.resources.ore -= this.blueprint.obsidianRobotOreCost;
        this.resources.clay -= this.blueprint.obsidianRobotClayCost;
        return this;
    }

    buyGeodeBot = () => {
        this.robots.geode++;
        this.resources.ore -= this.blueprint.geodeRobotOreCost;
        this.resources.obsidian -= this.blueprint.geodeRobotObsidianCost;
        return this;
    }

    canAffordOreBot = () => {
        return this.resources.ore >= this.blueprint.oreRobotOreCost;
    }

    canAffordClayBot = () => {
        return this.resources.ore >= this.blueprint.clayRobotOreCost;
    }

    canAffordObsidianBot = () => {
        return (this.resources.ore >= this.blueprint.obsidianRobotOreCost &&
            this.resources.clay >= this.blueprint.obsidianRobotClayCost);
    }

    canAffordGeodeBot = () => {
        return (this.resources.ore >= this.blueprint.geodeRobotOreCost &&
            this.resources.obsidian >= this.blueprint.geodeRobotObsidianCost);
    }

    // To prevent passing by reference...
    copy = () => {
        let step = new StepNode(this.blueprint, {...this.robots}, {...this.resources});
        step.minute = this.minute;
        return step;
    }
}

export default class BuildOrder{
    blueprints;
    
    constructor(){
        this.blueprints = [];
    }

    // Converts input into blueprint objects
    parseBlueprints = () => {
        // Blueprint 1: Each ore robot costs 4 ore. Each clay robot costs 2 ore. Each obsidian robot costs 3 ore and 14 clay. Each geode robot costs 2 ore and 7 obsidian.
        this.blueprints = this.blueprints.map(blueprint => {
            const id = parseInt(blueprint.match(/Blueprint [0-9]+/)[0].split(' ')[1]);
            const oreRobotOreCost = parseInt(blueprint.match(/Each ore robot costs [0-9]+ ore./)[0].split(' ')[4]);
            const clayRobotOreCost = parseInt(blueprint.match(/Each clay robot costs [0-9]+ ore./)[0].split(' ')[4]);
            const obsidianRobotOreCost = parseInt(blueprint.match(/Each obsidian robot costs [0-9]+ ore and [0-9]+ clay./)[0].split(' ')[4]);
            const obsidianRobotClayCost = parseInt(blueprint.match(/Each obsidian robot costs [0-9]+ ore and [0-9]+ clay./)[0].split(' ')[7]);
            const geodeRobotOreCost = parseInt(blueprint.match(/Each geode robot costs [0-9]+ ore and [0-9]+ obsidian./)[0].split(' ')[4]);
            const geodeRobotObsidianCost = parseInt(blueprint.match(/Each geode robot costs [0-9]+ ore and [0-9]+ obsidian./)[0].split(' ')[7]);

            return {
                id,
                oreRobotOreCost,
                clayRobotOreCost,
                obsidianRobotOreCost,
                obsidianRobotClayCost,
                geodeRobotOreCost,
                geodeRobotObsidianCost
            }
        })
    }

    // Thank piman for this algorithm
    // https://github.com/piman51277/AdventOfCode/tree/master/2022/19
    // Slow, but works.
    analyzeBlueprint = (minutes, initialStep) => {
        let stepQueue = [initialStep];
        let nextSteps = [];

        for (let minute = 0; minute < minutes; minute++){
            for (const step of stepQueue){
                // Build ore robot
                if (step.canAffordOreBot()){
                    nextSteps.push(step.copy().collect().buyOreBot());
                }
                // Build clay robot
                if (step.canAffordClayBot()){
                    nextSteps.push(step.copy().collect().buyClayBot());
                }
                // Build obsidian robot
                if (step.canAffordObsidianBot()){
                    nextSteps.push(step.copy().collect().buyObsidianBot());
                }
                // Build geode robot
                if (step.canAffordGeodeBot()){
                    nextSteps.push(step.copy().collect().buyGeodeBot());
                }

                // Don't build anything
                nextSteps.push(step.copy().collect());
            }

            // Determine score of upcoming paths
            nextSteps.forEach(step => {
                step.score = this.calculateScore(step, minutes);
            })

            // Sort and trim for best paths
            nextSteps = nextSteps.sort((a, b) => b.score - a.score).slice(0, 100000);
            // Assign to current and clear next
            stepQueue = nextSteps;
            nextSteps = [];
        }

        // Sort remaining paths
        stepQueue = stepQueue.sort((a, b) => b.resources.geode - a.resources.geode);

        // Return top path
        return stepQueue[0].resources.geode;
    }

    // Give a score to help determine value of build order
    calculateScore = (step, startingMinutes) => {
        // What's the possible number of future geodes?
        const futureGeodes = step.resources.geode + ((startingMinutes - step.minute) * step.robots.geode);

        const score = 
            futureGeodes * 10000000  +
            step.robots.obsidian * 10000 +
            step.robots.clay * 100 +
            step.robots.ore;

        return score;
    }

    // Looks for quality level of each blueprint and sums them
    getQuantityLevelsSum = (startingMinutes) => {
        // Define objects
        const robots = {
            ore: 1,
            clay: 0,
            obsidian: 0,
            geode: 0
        }

        const resources = {
            ore: 0,
            clay: 0,
            obsidian: 0,
            geode: 0
        }
        
        let total = 0;
        this.blueprints.forEach(blueprint => {
            let step = new StepNode(blueprint, {...robots}, {...resources});
            total += blueprint.id * this.analyzeBlueprint( 
                startingMinutes, 
                step
            );
        });

        // Return the sum of all quality levels
        return total;
    }

    // Examines only a set number of blueprints, returning product of geodes mined
    getGeodesSum = (startingMinutes, numberOfBlueprints = Infinity) => {
        // Define objects
        const robots = {
            ore: 1,
            clay: 0,
            obsidian: 0,
            geode: 0
        }

        const resources = {
            ore: 0,
            clay: 0,
            obsidian: 0,
            geode: 0
        }
        
        let total = 1;
        numberOfBlueprints = numberOfBlueprints > this.blueprints.length ? this.blueprints.length : numberOfBlueprints;
            

        for (let i = 0; i < numberOfBlueprints; i++){
            let step = new StepNode(this.blueprints[i], {...robots}, {...resources});
            total *= this.analyzeBlueprint( 
                startingMinutes, 
                step
            );
        }

        // Return the sum of all quality levels
        return total;
    }
}