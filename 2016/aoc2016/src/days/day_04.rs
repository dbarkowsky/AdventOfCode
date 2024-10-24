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
        // Add to the sum if the checksum matches
        if top_five_string.cmp(&instruction.checksum) == Ordering::Equal {
            id_sum += instruction.id;
        }
    }

    println!("{}", id_sum);
}

// Caesar cypher, move as many times as sector id
pub fn part_two() {
    const LOOKING_FOR: &str = "northpoleobjectstorage"; // Only knew this after first run
    let instructions: Vec<Instruction> = parse_input();
    for instruction in instructions {
        // Build a new string, using the hash
        const LETTERS_IN_ALPHABET: i32 = 26;
        const ASCII_START: i32 = 97; // lowercase a
        let move_amount = instruction.id % LETTERS_IN_ALPHABET; // Because 26 * 5 moves is the same as 26 moves
        let mut new_message = String::new();
        for mut letter in instruction.hash.chars() {
            let char_ascii = letter as i32; //
            let char_index = char_ascii - ASCII_START;
            // Can move forwards or backwards on ascii table depending on position
            // Goal is not to wrap around and have to deal with that logic
            if move_amount + char_index >= LETTERS_IN_ALPHABET {
                // Move backwards
                letter = char::from_u32((char_ascii - (LETTERS_IN_ALPHABET - move_amount)) as u32)
                    .unwrap();
            } else {
                // Move forwards
                letter = char::from_u32((char_ascii + move_amount) as u32).unwrap();
            }
            // Add to new string to construct message
            new_message.push_str(letter.to_string().as_str());
        }
        // Print and stop if we find it
        if new_message.eq(LOOKING_FOR) {
            println!("{}", instruction.id);
            break;
        }
    }
}

// derive piece allows for printing
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

// Convert the input list to a list of instructions
fn input_to_instruction(input: Vec<String>) -> Vec<Instruction> {
    // Regex to get groupings
    let pattern: Regex = Regex::new(r"(.*)([\d]{3})\[([a-z]+)\]").unwrap();
    return input
        .iter() // Convert to an iterable
        .map(|plain_input| {
            let temp: Vec<Instruction> = pattern
                .captures_iter(&plain_input)
                .filter_map(|mat| {
                    // This takes the matches and constructs an Instruction
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
