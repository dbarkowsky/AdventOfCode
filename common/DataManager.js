import fs from 'fs/promises';

export default class DataManager{
    // Returns file contents as string
    static loadData = async (filePath) => {
        return await fs.readFile(filePath, 'utf-8');
    }

    // Converts string to list with \r and \n removed
    static toList = (data) => {
        return data.split('\r\n');
    }

    // Loads data as string and coverts to list before return
    static loadDataToList = async (filePath) => {
        return (await fs.readFile(filePath, 'utf-8')).split('\r\n');
    }
}