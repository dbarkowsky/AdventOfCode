use crate::utils;

const DAY: &str = "03";
const TEST: bool = false;

// How many are true triangles? Two smallest sides > Biggest side
pub fn part_one() {
    let triangles = parse_input(true);
    let mut good_triangles = 0;
    for triangle in triangles {
        if triangle[0] + triangle[1] > triangle[2] {
            good_triangles += 1;
        }
    }
    println!("{}", good_triangles);
}

// Same, but have to assemble triangles vertically from input instead
pub fn part_two() {
    let original_triangles: Vec<Vec<i32>> = parse_input(false);
    let transposed_triangles: Vec<Vec<i32>> = transpose_input(original_triangles);
    let mut good_triangles = 0;
    for mut triangle in transposed_triangles {
        triangle.sort();
        if triangle[0] + triangle[1] > triangle[2] {
            good_triangles += 1;
        }
    }
    println!("{}", good_triangles);
}

fn transpose_input(original_input: Vec<Vec<i32>>) -> Vec<Vec<i32>> {
    let len = original_input[0].len();
    // For the length of one original row (3)
    // Converts rows/columns to columns/rows
    let mut initial_transpose = Vec::new();
    for i in 0..len {
        let mut temp_row = Vec::new();
        for j in 0..original_input.len() {
            temp_row.push(original_input[j][i]);
        }
        initial_transpose.push(temp_row);
    }

    // Split each group into 3s and flatten
    let mut flat_list = Vec::new();
    for row in initial_transpose {
        let mut index = 0;
        while index < row.len() {
            flat_list.push([row[index], row[index + 1], row[index + 2]].to_vec());
            index += 3;
        }
    }
    return flat_list;
}

fn parse_input(sort: bool) -> Vec<Vec<i32>> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    let mut number_input: Vec<Vec<i32>> = Vec::new();
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
        number_input.push(numbers);
    }
    return number_input;
}
