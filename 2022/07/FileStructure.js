// Represents a file or directory in the file structure
class Node{
    name; // name of file or dir
    size; // size in bytes
    directory; // boolean
    children; // A list of Nodes, if directory
    parent; // Another Node

    constructor(parent, name, directory = false){
        this.children = [];
        this.parent = parent;
        this.name = name;
        this.directory = directory;
    }

    // Sets the size of the node
    setSize = (size) => {this.size = parseInt(size);}

    // Calculates the size of the node. 
    // Recursive if node is a directory
    // If not a directory, just returns size
    calculateSize = () => {
        if (this.directory){
            this.size = 0;
            this.children.forEach(child => {
                if (child.directory)
                    this.size += child.calculateSize();
                else
                    this.size += child.size;
            });
            return this.size;
        }
        return this.size;
    }

    // Returns the child of this node with a matching name.
    // Throws exception if it doesn't have a child by that name.
    getChild = (name) => {
        let theChild;
        this.children.every(child => {
            if (child.name == name){
                theChild = child;
                return false;
            }
            return true;
        })

        if (!theChild)
            throw new Exception(`Child doesn't exist.`);
        return theChild;
    }
}

// Class for file structure of Elves' device
export default class FileStructure{
    root;
    currentDir;
    commands;
    allDirs;


    constructor(){
        this.root = new Node(null, '/', true);
        this.currentDir = this.root;
        this.commands = [];
        this.allDirs = [];
    }

    // Returns the size of a particular node, assume root if not specified
    getSize = (dirName = this.root) => {
        return dirName.calculateSize();
    }

    // Loops through commands and adds directories, files where needed, building tree
    processCommands = () => {
        let commandIndex = 0;
        while (commandIndex < this.commands.length){
            const command = this.commands[commandIndex];
            if (command.match(' cd ')){
                const [sign, cd, location] = command.split(' ');
                if (location == '/'){
                    this.currentDir = this.root;
                } else if (location == '..'){
                    if (this.currentDir.name !== '/')
                        this.currentDir = this.currentDir.parent;
                } else {
                    try{
                        this.currentDir = this.currentDir.getChild(location);
                    } catch (ex){
                        console.log(ex); // Child doesn't exist.
                    }    
                }
            } else if (command.match('[0-9]+ .+')){ // for files
                const [size, name] = command.split(' ');
                const newFile = new Node(this.currentDir, name);
                newFile.setSize(size);
                this.currentDir.children.push(newFile);
            } else if (command.match('dir .+')){ // for directories
                const [dir, name] = command.split(' ');
                const newDir = new Node(this.currentDir, name, true);
                this.currentDir.children.push(newDir);
                this.allDirs.push(newDir); // to track directories for part 2
            }

            commandIndex++;
        }    
    }

    // Returns the sum of all directories with a size less than threshold
    sumDirsUnder = (threshold) => {
        let sum = 0;
        this.allDirs.forEach(dir => {
            const size = dir.calculateSize();
            if (size < threshold){
                sum += size;
            }
        })
        return sum;
    }

    // Returns the directory node with the size closest to threshold without going under
    findSmallestDirOver = (threshold) => {
        let smallestDir = this.allDirs[0];
        this.allDirs.forEach(dir => {
            const size = dir.calculateSize();
            if (size > threshold && size < smallestDir.size){
                smallestDir = dir;
            }
        })
        return smallestDir;
    }
}