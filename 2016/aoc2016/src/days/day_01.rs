use crate::utils;
use std::num;

const DAY: &str = "01";
const TEST: bool = false;

enum Direction {
    North,
    East,
    South,
    West,
}

pub fn part_one() {
  let starting_coords: [i32; 2] = [0, 0];
    let mut current_coords: [i32; 2] = [0, 0];
    let mut direction = Direction::North;
    let input: Vec<String> = parse_input();
    for step in input {
        // Horrible way of getting turn and movement count
        let turn = &step[0..1];
        let count = &step[1..].parse::<i32>().unwrap();
        direction = get_new_direction(direction, turn);
        current_coords = move_elf(current_coords, *count, &direction);
    }
    let distance = get_manhattan_distance(current_coords[0], current_coords[1], starting_coords[0],starting_coords[1]);
    println!("{}", distance)
}

fn move_elf(current_coords: [i32; 2], distance: i32, direction: &Direction) -> [i32; 2] {
   let mut coord_copy = current_coords;
    match direction {
        Direction::East => coord_copy[0] += distance,
        Direction::North => coord_copy[1] += distance,
        Direction::South => coord_copy[1] -= distance,
        Direction::West => coord_copy[0] -= distance,
    }
    return coord_copy;
}

fn get_manhattan_distance(x1: i32, y1: i32, x2: i32, y2: i32) -> i32 {
  return (x2 - x1).abs() + (y2 - y1).abs();
}

fn get_new_direction(current: Direction, turn: &str) -> Direction {
    match current {
        Direction::East => {
            if turn.eq("R") {
                return Direction::South;
            } else {
                return Direction::North;
            };
        }
        Direction::South => {
            if turn.eq("R") {
                return Direction::West;
            } else {
                return Direction::East;
            };
        }
        Direction::North => {
            if turn.eq("R") {
                return Direction::East;
            } else {
                return Direction::West;
            };
        }
        Direction::West => {
            if turn.eq("R") {
                return Direction::North;
            } else {
                return Direction::South;
            };
        }
    }
}

fn parse_input() -> Vec<String> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    let instructions = input[0].split(", ").map(String::from).collect();
    return instructions;
}
