use crate::utils;

const DAY: &str = "02";
const TEST: bool = false;

pub fn part_one() {
    let input: Vec<String> = parse_input();
    let mut location = [0, 0];
    let min = 0;
    let max = 2;
    let mut answer = String::from("");
    let grid = [[1, 2, 3], [4, 5, 6], [7, 8, 9]];
    for row in input {
        let commands: Vec<String> = row
            .split("")
            .map(String::from)
            .filter(|command: &String| command != "")
            .collect();
        for command in commands {
            // Increment location
            match command.as_str() {
                "U" => {
                    if location[0] > min {
                        location[0] -= 1
                    };
                }
                "D" => {
                    if location[0] < max {
                        location[0] += 1
                    };
                }
                "L" => {
                    if location[1] > min {
                        location[1] -= 1
                    };
                }
                "R" => {
                    if location[1] < max {
                        location[1] += 1
                    };
                }
                &_ => todo!(),
            }
        }
        let value_on_grid: i32 = grid[location[0]][location[1]];
        answer.push_str(&value_on_grid.to_string());
    }
    println!("{}", answer);
}

fn parse_input() -> Vec<String> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    return input;
}
