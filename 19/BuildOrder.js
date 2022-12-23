const ORE = 0;
const CLAY = 1;
const OBSIDIAN = 2;
const GEODE = 3;

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

    // Recursive method to get highest geode count for a single blueprint
    // Purchases robots, mines resources, then starts any possible paths at this minute
    getHighestGeodeCount = (blueprint, remainingMinutes, robots, resources, purchase = undefined) => {
        // If 0 minutes left, return geode count
        if (remainingMinutes == 0){
            return resources.geode;
        }

        // Update robots based on last purchase
        switch (purchase) {
            case ORE:
                robots.ore++;
                break;
            case CLAY:
                robots.clay++;
                break;
            case OBSIDIAN:
                robots.obsidian++;
                break;
            case GEODE:
                robots.geode++;
                break;
            default: // Don't buy
                break;
        }
        
        // Copy original resources
        const originalResources = {...resources};

        // How many resources are gained? 
        resources.ore += robots.ore;
        resources.clay += robots.clay;
        resources.obsidian += robots.obsidian;
        resources.geode += robots.geode;

        const geodeCountsPerPath = [];
        // What buying paths are possible?
        // Can I buy an ore robot?
        if (originalResources.ore >= blueprint.oreRobotOreCost){
            const pathResources = {...resources};
            pathResources.ore -= blueprint.oreRobotOreCost;
            geodeCountsPerPath.push(this.getHighestGeodeCount(blueprint, --remainingMinutes, {...robots}, {...pathResources}, ORE));
        }
            
        // Can I buy a clay robot?
        if (originalResources.ore >= blueprint.clayRobotOreCost){
            const pathResources = {...resources};
            pathResources.ore -= blueprint.clayRobotOreCost;
            geodeCountsPerPath.push(this.getHighestGeodeCount(blueprint, --remainingMinutes, {...robots}, {...pathResources}, CLAY));
        }

        // Can I buy an obsidian robot?
        if (
            originalResources.ore >= blueprint.obsidianRobotOreCost && 
            originalResources.clay >= blueprint.obsidianRobotClayCost
        ){
            const pathResources = {...resources};
            pathResources.ore -= blueprint.obsidianRobotOreCost;
            pathResources.clay -= blueprint.obsidianRobotClayCost;
            geodeCountsPerPath.push(this.getHighestGeodeCount(blueprint, --remainingMinutes, {...robots}, {...pathResources}, OBSIDIAN));
        }

        // Can I buy a geode robot?
        if (
            originalResources.ore >= blueprint.geodeRobotOreCost &&
            originalResources.obsidian >= blueprint.geodeRobotObsidianCost
        ){
            const pathResources = {...resources};
            pathResources.ore -= blueprint.geodeRobotOreCost;
            pathResources.obsidian -= blueprint.geodeRobotObsidianCost;
            geodeCountsPerPath.push(this.getHighestGeodeCount(blueprint, --remainingMinutes, {...robots}, {...pathResources}, GEODE));
        }

        // What if I buy nothing?
        geodeCountsPerPath.push(this.getHighestGeodeCount(blueprint, --remainingMinutes, {...robots}, {...resources}));

        // Return the biggest geode count from all possible paths
        return geodeCountsPerPath.reduce((v1, v2) => v1 > v2 ? v1 : v2, 0);
    }

    getHighestWithQueue = (blueprint, startingMinutes, robots, resources) => {
        const buildQueue = [];
        let initialStep = {
            remainingMinutes: startingMinutes,
            robots,
            resources,
            purchase: undefined
        }
        buildQueue.push(initialStep);
        let maxGeodes = 0;

        while (buildQueue.length > 0){
            let currentStep = buildQueue.shift();
            let currentRobots = {...currentStep.robots};
            let currentResources = {...currentStep.resources};
            //console.log(currentStep.remainingMinutes)
            if (currentStep.remainingMinutes == 0){
                if (maxGeodes < currentResources.geode) maxGeodes = currentResources.geode;
            }

            // Update robots based on last purchase
            switch (currentStep.purchase) {
                case ORE:
                    currentRobots.ore++;
                    break;
                case CLAY:
                    currentRobots.clay++;
                    break;
                case OBSIDIAN:
                    currentRobots.obsidian++;
                    break;
                case GEODE:
                    currentRobots.geode++;
                    break;
                default: // Don't buy
                    break;
            }

            // Copy original resources
            const originalResources = {...currentResources};

            // How many resources are gained? 
            currentResources.ore += currentRobots.ore;
            currentResources.clay += currentRobots.clay;
            currentResources.obsidian += currentRobots.obsidian;
            currentResources.geode += currentRobots.geode;

            // Can I buy an ore robot?
            if (
                originalResources.ore >= blueprint.oreRobotOreCost && 
                currentStep.remainingMinutes > 0 &&
                currentRobots.ore * currentStep.remainingMinutes + currentResources.ore >= currentStep.remainingMinutes * blueprint.oreRobotOreCost
            ){
                const pathResources = {...currentResources};
                pathResources.ore -= blueprint.oreRobotOreCost;
                buildQueue.push({
                    remainingMinutes: currentStep.remainingMinutes - 1, 
                    robots: {...currentRobots},
                    resources: {...pathResources}, 
                    purchase: ORE});
            }
                
            // Can I buy a clay robot?
            if (
                originalResources.ore >= blueprint.clayRobotOreCost && 
                currentStep.remainingMinutes > 0 &&
                currentRobots.ore * currentStep.remainingMinutes + currentResources.ore >= currentStep.remainingMinutes * blueprint.clayRobotOreCost
            ){
                const pathResources = {...currentResources};
                pathResources.ore -= blueprint.clayRobotOreCost;
                buildQueue.push({
                    remainingMinutes: currentStep.remainingMinutes - 1, 
                    robots: {...currentRobots},
                    resources: {...pathResources}, 
                    purchase: CLAY});            }

            // Can I buy an obsidian robot?
            if (
                originalResources.ore >= blueprint.obsidianRobotOreCost && 
                originalResources.clay >= blueprint.obsidianRobotClayCost  && 
                currentStep.remainingMinutes > 0 &&
                currentRobots.ore * currentStep.remainingMinutes + currentResources.ore >= currentStep.remainingMinutes * blueprint.obsidianRobotOreCost &&
                currentRobots.clay * currentStep.remainingMinutes + currentResources.clay >= currentStep.remainingMinutes * blueprint.obsidianRobotClayCost
            ){
                const pathResources = {...currentResources};
                pathResources.ore -= blueprint.obsidianRobotOreCost;
                pathResources.clay -= blueprint.obsidianRobotClayCost;
                buildQueue.push({
                    remainingMinutes: currentStep.remainingMinutes - 1, 
                    robots: {...currentRobots},
                    resources: {...pathResources}, 
                    purchase: OBSIDIAN});            }

            // Can I buy a geode robot?
            if (
                originalResources.ore >= blueprint.geodeRobotOreCost &&
                originalResources.obsidian >= blueprint.geodeRobotObsidianCost && 
                currentStep.remainingMinutes > 0 && 
                currentRobots.ore * currentStep.remainingMinutes + currentResources.ore >= currentStep.remainingMinutes * blueprint.geodeRobotOreCost &&
                currentRobots.obsidian * currentStep.remainingMinutes + currentResources.obsidian >= currentStep.remainingMinutes * blueprint.geodeRobotObsidianCost
            ){
                const pathResources = {...currentResources};
                pathResources.ore -= blueprint.geodeRobotOreCost;
                pathResources.obsidian -= blueprint.geodeRobotObsidianCost;
                buildQueue.push({
                    remainingMinutes: currentStep.remainingMinutes - 1, 
                    robots: {...currentRobots},
                    resources: {...pathResources}, 
                    purchase: GEODE});            
            }

            // Don't buy anything
            if (currentStep.remainingMinutes > 0){
                const pathResources = {...currentResources};
                buildQueue.push({
                    remainingMinutes: currentStep.remainingMinutes - 1, 
                    robots: {...currentRobots},
                    resources: {...pathResources}, 
                    purchase: undefined});  
            }
        }

        return maxGeodes;
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
        
        // For holding quality levels that are returned
        const qualityLevels = [];

        this.blueprints.forEach(blueprint => {
            // qualityLevels.push(blueprint.id * this.getHighestGeodeCount(
            //     blueprint, 
            //     startingMinutes, 
            //     {...robots},
            //     {...resources}
            // ));
            qualityLevels.push(blueprint.id * this.getHighestWithQueue(
                blueprint,
                startingMinutes,
                {...robots},
                {...resources}
            ))
        })

        // Return the sum of all quality levels
        return qualityLevels.reduce((v1, v2) => v1 += v2, 0);
    }
}