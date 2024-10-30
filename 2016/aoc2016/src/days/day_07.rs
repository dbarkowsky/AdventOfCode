use std::{cmp::Ordering, collections::HashMap};
use crate::utils;

const DAY: &str = "07";
const TEST: bool = false; 

// Part 1 answer for test data = 2
pub fn part_one() {
    let input: Vec<HashMap<String, Vec<String>>> = parse_input();
    let mut tls_count = 0;
    for line in input {
      let inside_abba_count = count_abba_in_list(line.get("inside").unwrap().to_vec());
      let outside_abba_count = count_abba_in_list(line.get("outside").unwrap().to_vec());
      // println!("{:#?}", line);
      // println!("{:#?}", outside_abba_count);
      // println!("{:#?}", inside_abba_count);

      if outside_abba_count >= 1 && inside_abba_count == 0 {
        tls_count += 1;
      }
    }
    println!("{}", tls_count);
}

pub fn part_two() {
}

fn count_abba_in_list(list: Vec<String>) -> i32 {
  let mut count = 0;
  for element in list {
    // Check if it contains a palindrome
    if has_palindrome(element){
      count += 1;
    }
  }
  return count;
}

fn has_palindrome(segment: String) -> bool {
  let letters: Vec<char> = segment.chars().into_iter().collect();
  // println!("{:#?}", letters);
  for i in 0..(segment.len() - 3) {
    if letters[i] == letters[i + 3] && letters[i + 1] == letters[i + 2] && letters[i] != letters[i+1]{
      return true;
    }
  }
  return false;
}

fn parse_input()  -> Vec<HashMap<String, Vec<String>>>{
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    let mut separated_input: Vec<HashMap<String, Vec<String>>> = Vec::new();
    for line in input {
      let segments: Vec<&str> = line.split(|character| character == '[' || character == ']').collect();
      let mut outside_segments = Vec::new();
      let mut inside_segments = Vec::new();
      let mut line_map = HashMap::new();
      for i in 0..segments.len() {
        if i % 2 == 0 {
          outside_segments.push(String::from(segments[i]));
        } else {
          inside_segments.push(String::from(segments[i]));
        }
      }
      line_map.insert(String::from("outside"), outside_segments);
      line_map.insert(String::from("inside"), inside_segments);
      separated_input.push(line_map);
    }
    return separated_input;
}
