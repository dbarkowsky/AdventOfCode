export default class BuildOrder{
    blueprints;
    
    constructor(){
        this.blueprints = [];
    }

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

    getOptimalBuildOrder = (blueprint, remainingMinutes, robots, resources, purchase = undefined) => {

    }

    getQuantityLevels = () => {

    }
}