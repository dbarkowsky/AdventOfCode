export default class Valves{
    valves;
    startingValve;
    rawInput;
    openValves;

    constructor() {
        this.rawInput = [];
        this.valves = {};
    }

    // Originally tried BFS search with weight. Also tried to use every possible combo.
    // Neither of those worked so...
    // Guided attempt based on u/frhel's solution

    // Converts the input into valve objects.
    // Gets the shortest possible path to each other valve, and stores those values
    buildValves = () => {
        this.rawInput.forEach(line => {
            const name = line.substring(line.indexOf('Valve') + 6, line.indexOf('has') - 1);
            const rate = parseInt(line.substring(line.indexOf('rate=') + 5, line.indexOf(';')));
            let tunnels = line.substring(line.indexOf('valve', line.indexOf(';')) + 5);
            if (tunnels[0] == 's') tunnels = tunnels.substring(2);
            if (tunnels[0] == ' ') tunnels = tunnels.substring(1);
            tunnels = tunnels.split(', ');
            this.valves[name] = {name, rate, tunnels};
        })

        // Convert valves to list [name, {name, rate, tunnels}]
        // Filter out valves with 0 rate
        let valveList = [...Object.entries(this.valves)].filter(valve => valve[1].rate > 0 || valve[1].name == 'AA');

        // Get shortest paths to each other valve
        valveList.forEach(entry => {
            entry[1].paths = this.getShortestPaths(entry[0]);
        })
        
        // Rebuild back into object
        this.valves = {};
        valveList.forEach(valve => this.valves[valve[0]] = valve[1]);
        console.log(this.valves)
    }

    // Uses BFS to find shortest path
    getShortestPaths = (startingValveName) => {
        let paths = {};
        let valveList = [...Object.entries(this.valves)];
        valveList.forEach(entry => {
            // Don't calculate if valve is starting valve or if valve has no flow rate
            if (!(entry[1].name == startingValveName || entry[1].rate == 0)){
                const visitedValves = [];
                const toVisitQueue = [startingValveName];
                const path = [];
                // While some valves haven't been visited yet
                while (toVisitQueue.length > 0){
                    const currentValveName = toVisitQueue.shift();
                    const currentValveObj = this.valves[currentValveName];
                    
                    // If this valve connects directly to the current valve being searched for, 
                    // trace a path back to the starting valve to determine distance
                    if (currentValveObj.tunnels.includes(entry[1].name)){
                        paths[entry[1].name] = this.tracePath(path, currentValveName, entry[1].name);
                        break;
                    }

                    // Add other valves to queue if they haven't been visited
                    currentValveObj.tunnels.forEach(tunnel => {
                        if (!visitedValves.includes(tunnel)){
                            path.push({tunnel, name: currentValveName});
                            toVisitQueue.push(tunnel);
                        }
                    })

                    visitedValves.push(currentValveName);
                }
            }
                
        })
        return paths;
    }

    // Unwinds the supplied path, building the return path and returning its length
    tracePath = (path, currentValve, endingValve) => {
        let returnPath = [endingValve, currentValve];
        while (path.length > 0){
            let current = path.pop();
            if (current.tunnel == currentValve){
                returnPath.push(current.name);
                currentValve = current.name;
            }
        }

        return returnPath.length;
    }

    // Finds the path that will release the most pressure given the time and valves provided
    findBestPath = (startingValveName, minutes, valves = this.valves) => {
        const pathStack = [];
        pathStack.push({
            valve: valves[startingValveName],
            visited: [startingValveName],
            steps: 0, // minutes that have passed so far
            flow: 0, // The flow that happens during a move (total flow * minutes passed moving)
            flowRate: 0, // The flow rate per minute
            flowSum: 0 // The total flow that has flown
        })

        let bestPath = pathStack[0]; // starting best '0'

        while (pathStack.length > 0){
            const currentPath = pathStack.pop();
            currentPath.visited.push(currentPath.valve.name);

            // Is this path better than our current best?
            bestPath = bestPath.flowSum > currentPath.flowSum ? bestPath : currentPath;

            let valveList = [...Object.values(valves)];
            for (const valve of valveList){
                // Skip if we've already visited node, it's the node we're on, or if it's too far away
                if (
                    currentPath.valve.name == valve.name ||
                    currentPath.steps + currentPath.valve.paths[valve.name] > minutes ||
                    currentPath.visited.includes(valve.name)
                ){
                    continue;
                } 

                // Update currentPath's steps and flowSum
                // Use copy so original isn't modified
                const currentPathCopy = {...currentPath};
                currentPathCopy.steps = currentPath.steps + currentPath.valve.paths[valve.name];
                currentPathCopy.flow = (currentPath.flowRate * currentPath.valve.paths[valve.name]) + currentPath.flow;
                currentPathCopy.flowRate += valve.rate;
                currentPathCopy.flowSum = (currentPathCopy.flowRate * (minutes - currentPathCopy.steps)) + currentPathCopy.flow;

                // If the total flow is less than bestFlow, not worth adding to stack
                // else add back to stack
                if (
                    !(currentPathCopy.flowSum < bestPath.flowSum &&
                      currentPathCopy.steps >= bestPath.steps)
                )
                {
                    pathStack.push({
                        valve: valve, // Next valve to be checked
                        visited: [...currentPathCopy.visited],
                        steps: currentPathCopy.steps,
                        flow: currentPathCopy.flow,
                        flowRate: currentPathCopy.flowRate,
                        flowSum: currentPathCopy.flowSum
                    });
                }
            }
        }

        return bestPath.flowSum;
    }

    // Assumes you and an elephant are now opening valves, hardcoded for 2
    // Separates valves into two paths, gets all possible outcomes and picks best.
    workWithElephant = (staringValveName, minutes) => {
        const startingValve = this.valves[staringValveName];

        // Create list of unopened valves
        // Omit starting valve
        const unopenedValveList = [...Object.values(this.valves)].filter(valve => valve.name != staringValveName);

        // Create two paths
        const twoPaths = [[], []];
        const valvesPerPath = Math.floor(unopenedValveList.length / 2);
        const remainingValves = unopenedValveList.length % 2;
        // Get all combinations for each set of valves
        twoPaths[0] = this.combinations(unopenedValveList, valvesPerPath);
        twoPaths[1] = this.combinations(unopenedValveList, valvesPerPath + remainingValves); // give the elephant the extra valves...

        // Flesh out path objects
        for (let i = 0; i < twoPaths.length; i++){
            for (let j = 0; j < twoPaths[i].length; j++){
                // Add starting valve
                twoPaths[i][j].push(startingValve);
                // Split that entry into a list of valve names (path) and the object
                twoPaths[i][j] = {
                    path: twoPaths[i][j].map(valve => valve.name),
                    valves: twoPaths[i][j]
                };
            }
        }

        let bestFlow = []; // Put all results here, choose best later

        // For each possible combination
        for (let i = 0; i < twoPaths[1].length; i++){
            for (let j = 0; j < twoPaths[0].length; j++){
                // Filter out the shared valves between the two paths,
                // We don't need duplicates
                const sharedValves = twoPaths[1][i].path.filter(valveName => twoPaths[0][j].path.includes(valveName));
                // If there's only 1, only the start node is there, so it's ok to proceed
                // Otherwise, skip this iteration
                if (sharedValves.length !== 1)
                    continue;

                // Create object of valves for me to open
                const myValves = {};
                twoPaths[0][i].valves.forEach(valve => {
                    myValves[valve.name] = valve;
                })

                // Create object of valves for elephant
                const elephantValves = {};
                twoPaths[1][j].valves.forEach(valve => {
                    elephantValves[valve.name] = valve;
                })

                // Get their combined flow and add to list
                const combinedFlow = this.findBestPath('AA', minutes, myValves) + this.findBestPath('AA', minutes, elephantValves);
                bestFlow.push(combinedFlow)
            }
        }

        return bestFlow.sort((a, b) => b - a)[0];
    }

    // This one was borrowed without much change.
    // It recursively finds all possible combinations given the passed array of valves
    // Only goes as deep as the length allows.
    // I have reviewed this thoroughly and still find it confusing
    combinations = (arr, len) => {
        if (len === 0) return [[]];
        const result = [];
        for (let i = 0; i <= arr.length - len; i++) {
            const recursedResult = this.combinations(arr.slice(i + 1), len - 1);
            for (const combination of recursedResult) {
                result.push([arr[i], ...combination]);
            }
        }
        return result;
    }
}