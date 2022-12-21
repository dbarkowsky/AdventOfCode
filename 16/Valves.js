export default class Valves{
    valves;
    startingValve;
    rawInput;
    openValves;

    constructor() {
        this.rawInput = [];
        this.valves = {};
        this.startingValve = 'AA';
        this.openValves = new Set();
    }

    parseInput = () => {
        // Example input: Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
        this.rawInput.forEach(line => {
            const valveName = line.substring(line.indexOf('Valve') + 6, line.indexOf('has') - 1);
            const flow = parseInt(line.substring(line.indexOf('rate=') + 5, line.indexOf(';')));
            let tunnels = line.substring(line.indexOf('valve', line.indexOf(';')) + 5);
            if (tunnels[0] == 's') tunnels = tunnels.substring(2);
            if (tunnels[0] == ' ') tunnels = tunnels.substring(1);
            tunnels = tunnels.split(', ');
            
            this.valves[valveName] = {
                name: valveName,
                flow: flow,
                tunnels: tunnels
            }
        });
    }

    findGreatestPressure = (currentValve, minutesRemaining, pressureSoFar, firstNode = false) => {
        //console.log(minutesRemaining);
        if (minutesRemaining == 0)
            return pressureSoFar;

        let maxIfOpeningValve = 0;
        let maxIfValveAlreadyOpenOrUntouched = 0;
        
        // If Valve is already open OR valve is closed, but we don't open it to save 
        if (this.openValves.has(this.valves[currentValve].name) || this.valves[currentValve].flow == 0){
            maxIfValveAlreadyOpenOrUntouched = this.valves[currentValve].tunnels.reduce((maxPressure, currentTunnel) => Math.max(
                maxPressure, 
                this.findGreatestPressure(currentTunnel, minutesRemaining - 1, pressureSoFar)
                ), -1);
        }
        
        // If valve is closed, and we want to open it
        if (!this.openValves.has(this.valves[currentValve].name) && this.valves[currentValve].flow != 0 && minutesRemaining > 0){
            this.openValves.add(this.valves[currentValve].name);
            minutesRemaining--;
            pressureSoFar += this.valves[currentValve].flow * minutesRemaining;
            if (minutesRemaining == 0){
                return pressureSoFar;
            }
            maxIfOpeningValve = this.valves[currentValve].tunnels.reduce((maxPressure, currentTunnel) => Math.max(
                maxPressure, 
                this.findGreatestPressure(currentTunnel, minutesRemaining - 1, pressureSoFar)
                ), -1);
        }
        
        return pressureSoFar + Math.max(maxIfOpeningValve, maxIfValveAlreadyOpenOrUntouched);
    }
}