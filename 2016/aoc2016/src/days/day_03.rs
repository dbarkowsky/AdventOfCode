use crate::utils;

const DAY: &str = "03";
const TEST: bool = true;

pub fn part_one() {
    let triangles = parse_input(true);
    let mut goodTriangles = 0;
    for triangle in triangles {
        if (triangle[0] + triangle[1] > triangle[2]) {
            goodTriangles += 1;
        }
    }
    println!("{}", goodTriangles);
}

pub fn part_two() {
    let originalTriangles: Vec<Vec<i32>> = parse_input(false);
    let transposedTriangles: Vec<Vec<i32>> = transpose_input(originalTriangles);
    println!("{:#?}", transposedTriangles);
}

fn transpose_input(originalInput: Vec<Vec<i32>>) -> Vec<Vec<i32>> {
    let len = originalInput[0].len();
    // For the length of one original row (3)
    // Converts rows/columns to columns/rows
    let mut initialTranspose = Vec::new();
    for i in 0..len {
        let mut tempRow = Vec::new();
        for j in 0..originalInput.len() {
            tempRow.push(originalInput[j][i]);
        }
        initialTranspose.push(tempRow);
    }

    // Split each group into 3s and flatten
    let mut flatList = Vec::new();
    for row in initialTranspose {
        let mut index = 0;
        while index < row.len() {
            flatList.push([row[index], row[index + 1], row[index + 2]].to_vec());
            index += 3;
        }
    }
    return flatList;
}

fn parse_input(sort: bool) -> Vec<Vec<i32>> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    let mut numberInput: Vec<Vec<i32>> = Vec::new();
    for row in input {
        // This is some real confusing logic for how to filter. Pain with referencing and dereferencing.
        let mut numbers: Vec<i32> = row
            .split(" ")
            .filter(|item: &&str| *item != "")
            .map(|s: &str| s.parse::<i32>().unwrap())
            .collect();
        if sort {
            numbers.sort()
        };
        numberInput.push(numbers);
    }
    return numberInput;
}
