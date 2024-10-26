use std::collections::HashMap;
use crate::utils;

const DAY: &str = "06";
const TEST: bool = false;

// Find most common letter in each row of transposed data
pub fn part_one() {
    let input: Vec<Vec<String>> = parse_input();
    let transposed_input = transpose_input(input);

    println!("{}", get_answer(transposed_input, Order::DESC));
}

// Same thing, but least common letter
pub fn part_two() {
    let input: Vec<Vec<String>> = parse_input();
    let transposed_input = transpose_input(input);

    println!("{}", get_answer(transposed_input, Order::ASC));
}

fn parse_input() -> Vec<Vec<String>> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    let split_input = input
        .iter()
        .map(|line| {
            line.split("")
                .filter(|item: &&str| *item != "")
                .map(|character| character.to_string())
                .collect()
        })
        .collect();
    return split_input;
}

// Add this derive so we can use == later
#[derive(PartialEq, Eq)]
enum Order {
    ASC,
    DESC,
}

fn get_answer(transposed_input: Vec<Vec<String>>, sort_order: Order) -> String {
    let mut answer = String::new();

    // We've done this in Day 04
    for row in transposed_input {
        // Count the frequency of letters in the row
        let mut letter_count: HashMap<char, i32> = HashMap::new();
        for j in 0..row.len() {
            let letter: Vec<char> = row[j].chars().collect();
            // Get the entry or set it to 0 if doesn't exist
            let count = letter_count.entry(letter[0]).or_insert(0);
            // Increment the value at this address.
            *count += 1;
        }

        // Order the counts
        // Sort the letter_count by values
        let mut sorted_counts: Vec<(&char, &i32)> = letter_count.iter().collect();
        // .1 is the value, .0 is the key
        if sort_order == Order::ASC {
            sorted_counts.sort_by(|a, b| a.1.cmp(b.1));
        } else if sort_order == Order::DESC {
            sorted_counts.sort_by(|a, b| b.1.cmp(a.1));
        }

        // Add to answer
        answer.push(*sorted_counts[0].0);
    }
    return answer;
}

// Borrowed from day 3
fn transpose_input(input: Vec<Vec<String>>) -> Vec<Vec<String>> {
    let len = input[0].len();
    // For the length of one original row
    // Converts rows/columns to columns/rows
    let mut input_transposed = Vec::new();
    for i in 0..len {
        let mut temp_row = Vec::new();
        for j in 0..input.len() {
            temp_row.push(input[j][i].clone());
        }
        input_transposed.push(temp_row);
    }
    return input_transposed;
}
