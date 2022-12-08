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

    setSize = (size) => {this.size = size;}

    calculateSize = () => {
        this.size = 0;
        this.children.forEach(child => {
            if (child.directory)
                this.size += child.calculateSize();
            else
                this.size += child.size;
        });
        return this.size; // if used recursively
    }


}

export default class FileStructure{
    root;
    currentDir;
    commands;

    constructor(){
        this.root = new Node(null, '/', true);
        this.currentDir = this.root;
        this.commands = [];
    }

    getSize = (dirName = this.root) => {
        return dirName.calculateSize();
    }

    processCommands = () => {
        
    }
}