export default class ElfAssignments{
    shifts;
    constructor(){
        this.shifts = [];
    }

    // splits the shift pairs from one string to two list items
    splitShifts = () => {
        this.shifts = this.shifts.map(shift => shift.split(','))
    }

    // checks if two string ranges (e.g. 3-6, 5-6) overlap fully
    overlapFully = (rangeA, rangeB) => {
        const listA = this.getRangeList(rangeA);
        const listB = this.getRangeList(rangeB);

        if (
            listA.every(num => listB.includes(num)) || 
            listB.every(num => listA.includes(num))
        )
            return true;
        return false;
    }

    // Checks if two string ranges (e.g. 3-6, 5-6) overlap partially
    overlapPartially = (rangeA, rangeB) => {
        const listA = this.getRangeList(rangeA);
        const listB = this.getRangeList(rangeB);

        for (let i = 0; i < listA.length; i++){
            if (listB.includes(listA[i]))
                return true;
        }
        return false;
    }

    // Converts string range to list form
    getRangeList = (range) => {
        const [start, end] = range.split('-').map(number => parseInt(number));
        const list = [];

        for (let i = start; i <= end; i++){
            list.push(i);
        }
        return list;
    }

    // Counts amount of overlap of shifts
    countOverlap = (onlyFully = false) => {
        let count = 0;
        this.shifts.forEach(shift => {
            if (
                // Forgive me for this
                onlyFully 
                    ? this.overlapFully(shift[0], shift[1]) 
                    : this.overlapPartially(shift[0], shift[1])
                ){
                count++;
            }
        });
        return count;
    }
}