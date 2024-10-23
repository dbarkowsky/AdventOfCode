use std::{cmp::Ordering, collections::HashMap};

use crate::utils;
use regex::Regex;

const DAY: &str = "04";
const TEST: bool = false;

// What's the sum of ids from all real rooms (where checksum matches most frequent letters)?
pub fn part_one() {
    let instructions: Vec<Instruction> = parse_input();
    let mut id_sum = 0;
    // For each instruction
    for instruction in instructions {
        // Count the frequency of letters in the hash
        let mut letter_count: HashMap<char, i32> = HashMap::new();
        for j in 0..instruction.hash.len() {
            let letter = instruction.hash.chars().nth(j).unwrap();
            // Get the entry or set it to 0 if doesn't exist
            let count = letter_count.entry(letter).or_insert(0);
            // Increment the value at this address.
            *count += 1;
        }

        // Order the counts from greatest to least
        // Sort the letter_count by values then by keys
        let mut sorted_counts: Vec<(&char, &i32)> = letter_count.iter().collect();
        // .1 is the value, .0 is the key
        sorted_counts.sort_by(|a, b| b.1.cmp(a.1).then_with(|| a.0.cmp(b.0)));

        // See if the top N of the counts matches the checksum
        let checksum_length = instruction.checksum.len();
        let mut top_five_string = "".to_owned();
        for i in 0..checksum_length {
            top_five_string.push_str(&sorted_counts[i].0.to_string());
        }
        if top_five_string.cmp(&instruction.checksum) == Ordering::Equal {
            id_sum += instruction.id;
        }
    }

    println!("{}", id_sum);
}

// Same, but have to assemble triangles vertically from input instead
pub fn part_two() {}

#[derive(Debug)]
struct Instruction {
    id: i32,
    checksum: String,
    hash: String,
}

fn parse_input() -> Vec<Instruction> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    return input_to_instruction(input);
}

fn input_to_instruction(input: Vec<String>) -> Vec<Instruction> {
    let pattern: Regex = Regex::new(r"(.*)([\d]{3})\[([a-z]+)\]").unwrap();
    return input
        .iter()
        .map(|plain_input| {
            let temp: Vec<Instruction> = pattern
                .captures_iter(&plain_input)
                .filter_map(|mat| {
                    let groups = (mat.get(1), mat.get(2), mat.get(3));
                    match groups {
                        (Some(hash), Some(id), Some(checksum)) => {
                            let parsed_id = id.as_str().parse::<i32>().unwrap_or_default();
                            Some(Instruction {
                                id: parsed_id,
                                checksum: checksum.as_str().to_string(),
                                hash: hash.as_str().replace("-", "").to_string(),
                            })
                        }
                        _ => None,
                    }
                })
                .collect();
            return temp;
        })
        .flatten()
        .collect();
}
