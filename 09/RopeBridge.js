export default class RopeBridge{
    rope; // Array containing head, tail, and x number of segments in between
    head; // Head of rope, object with x and y
    tail; // Tail of rope, object with x and y
    instructions; // Instructions for movement of head
    tailLocations; // A set holding each unique tail location

    constructor(lengthOfMidSegments){
        this.rope = new Array(lengthOfMidSegments + 2);
        this.populateRope();
        this.head = this.rope[0];
        this.tail = this.rope[this.rope.length - 1];
        this.instructions = [];

        // Start automatically visited by tail
        this.tailLocations = new Set();
        this.tailLocations.add(JSON.stringify(this.tail));
    }

    // Iterates through instructions, moving the head as directed
    giveInstructions = () => {
        this.instructions.forEach(instruction => {
            //console.log(instruction);
            const [direction, distance] = instruction.split(' ');
            this.moveHead(direction, parseInt(distance));
        })
    }

    // Populates the rope with starting objects.
    populateRope = () => {
        for (let i = 0; i < this.rope.length; i++){
            this.rope[i] = this.createObject(0, 0);
        }
    }

    // Returns an object for rope segment location
    createObject(x, y){
        return {
            x: x,
            y: y,
        }
    }

    // Moves the head according to direction and distance, then moves each following rope segment
    moveHead = (direction, distance) => {
        switch (direction){
            case 'U':
                for (let i = 0; i < distance; i++){
                    this.head.y += 1;
                    for (let i = 1; i < this.rope.length; i++){
                        this.moveSegments(this.rope[i - 1], this.rope[i]);
                    }
                }
                break;
            case 'D':
                for (let i = 0; i < distance; i++){
                    this.head.y -= 1;
                    for (let i = 1; i < this.rope.length; i++){
                        this.moveSegments(this.rope[i - 1], this.rope[i]);
                    }
                }
                break;
            case 'L':
                for (let i = 0; i < distance; i++){
                    this.head.x -= 1;
                    for (let i = 1; i < this.rope.length; i++){
                        this.moveSegments(this.rope[i - 1], this.rope[i]);
                    }
                }
                break;
            case 'R':
                for (let i = 0; i < distance; i++){
                    this.head.x += 1;
                    for (let i = 1; i < this.rope.length; i++){
                        this.moveSegments(this.rope[i - 1], this.rope[i]);
                    }
                }
                break;
        }
        //console.log('head:', `${this.head.x},${this.head.y}`, 'tail:', `${this.tail.x},${this.tail.y}`);
    }

    // Moves a segment with regard to its previous segment.
    moveSegments = (precedingSegment, currentSegment) => {
        // If more than 1 away both horizontally and vertically (should never be more than 2)
        if (Math.abs(precedingSegment.x - currentSegment.x) > 1 && Math.abs(precedingSegment.y - currentSegment.y) > 1){
            currentSegment.x += Math.ceil((precedingSegment.x - currentSegment.x)/2);
            currentSegment.y += Math.ceil((precedingSegment.y - currentSegment.y)/2);;
        }

        // If more than 1 horizontally, but only 1 vertically away
        if (Math.abs(precedingSegment.x - currentSegment.x) > 1 && Math.abs(precedingSegment.y - currentSegment.y) == 1){
            currentSegment.x += Math.ceil((precedingSegment.x - currentSegment.x)/2);
            currentSegment.y += precedingSegment.y - currentSegment.y;
        }

        // If more than 1 vertically, but only 1 horizontally away
        if (Math.abs(precedingSegment.y - currentSegment.y) > 1 && Math.abs(precedingSegment.x - currentSegment.x) == 1){
            currentSegment.x += precedingSegment.x - currentSegment.x;
            currentSegment.y += Math.ceil((precedingSegment.y - currentSegment.y)/2);
        }

        // If more than 1 horizontally, but on same vertical plane
        if (Math.abs(precedingSegment.x - currentSegment.x) > 1 && Math.abs(precedingSegment.y - currentSegment.y) == 0){
            currentSegment.x += Math.ceil((precedingSegment.x - currentSegment.x)/2);
        }

        // If more than 1 vertically, but on same horizontal plane
        if (Math.abs(precedingSegment.y - currentSegment.y) > 1 && Math.abs(precedingSegment.x - currentSegment.x) == 0){
            currentSegment.y += Math.ceil((precedingSegment.y - currentSegment.y)/2);
        }

        // Try to add the tail to the set
        this.tailLocations.add(JSON.stringify(this.tail));
    }
}
