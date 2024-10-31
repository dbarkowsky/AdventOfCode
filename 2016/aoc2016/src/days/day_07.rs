use std::collections::HashMap;
use crate::utils;

const DAY: &str = "07";
const TEST: bool = false; 
// Started using references on this one. Need to keep this in mind going forward.
// Made things easier. Need to remember my C++.

// Part 1 answer for test data = 2
// How many have abba outside but no abba inside?
pub fn part_one() {
    let input: Vec<HashMap<String, Vec<String>>> = parse_input();
    let mut tls_count: i32 = 0;
    for line in input {
      let inside_abba_count: i32 = count_abba_in_list(line.get("inside").unwrap().to_vec());
      let outside_abba_count: i32 = count_abba_in_list(line.get("outside").unwrap().to_vec());

      if outside_abba_count >= 1 && inside_abba_count == 0 {
        tls_count += 1;
      }
    }
    println!("{}", tls_count);
}

// Part 2 answer for test data = 3
// How many have bab inside when having aba outside?
pub fn part_two() {
  let input: Vec<HashMap<String, Vec<String>>> = parse_input();
    let mut ssl_count: i32 = 0;
    for line in input {
      let aba_list: Vec<String> = get_abas_in_list(line.get("outside").unwrap().to_vec());
      if has_reverse_aba(&aba_list, &line.get("inside").unwrap().to_vec()) {
        ssl_count += 1;
      }
    }
    println!("{}", ssl_count);
}

// Does a list have any of the reversed abas found in other list?
// Uses references to avoid ownership issues
fn has_reverse_aba(list_of_abas: &Vec<String>, list_to_check_against: &Vec<String>) -> bool {
  for aba in list_of_abas {
    let reverse_aba: String = reverse_aba(&aba);
    for list_segment in list_to_check_against {
      if list_segment.contains(&reverse_aba.as_str()) {
        return true;
      }
    }
  }
  return false;
}

// Just reverses a 3-character string.
fn reverse_aba(aba: &String) -> String {
  let letters: Vec<char> = aba.chars().into_iter().collect();
  return String::from_iter([letters[1], letters[0], letters[1]].iter()) 
}

// Returns all 3-character palindromes in a list of strings
fn get_abas_in_list(list: Vec<String>) -> Vec<String> {
  let mut aba_list: Vec<String> = Vec::new();
  for segment in list {
    let letters: Vec<char> = segment.chars().into_iter().collect();
    for i in 0..(segment.len() - 2) {
      if letters[i] == letters[i + 2] && letters[i] != letters[i+1]{
        aba_list.push([letters[i], letters[i + 1], letters[i + 2]].iter().collect());
      }
    }
  }
  return aba_list;
}

// Gets counts of 4-character palindromes
fn count_abba_in_list(list: Vec<String>) -> i32 {
  let mut count: i32 = 0;
  for element in list {
    // Check if it contains a palindrome
    if has_palindrome(element){
      count += 1;
    }
  }
  return count;
}

// We're only interested in 4-character long palindromes
// Outside and inside letters must be different
fn has_palindrome(segment: String) -> bool {
  let letters: Vec<char> = segment.chars().into_iter().collect();
  for i in 0..(segment.len() - 3) {
    if letters[i] == letters[i + 3] && letters[i + 1] == letters[i + 2] && letters[i] != letters[i+1]{
      return true;
    }
  }
  return false;
}

fn parse_input()  -> Vec<HashMap<String, Vec<String>>>{
    let test_suffix: &str = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    // Convert these lines to a hash map list with lists of outside and inside strings
    let mut separated_input: Vec<HashMap<String, Vec<String>>> = Vec::new();
    for line in input {
      // Split on [ or ]
      let segments: Vec<&str> = line.split(|character| character == '[' || character == ']').collect();
      let mut outside_segments: Vec<String> = Vec::new();
      let mut inside_segments: Vec<String> = Vec::new();
      let mut line_map: HashMap<String, Vec<String>> = HashMap::new();
      // All even segments are outside, all odd are inside
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
