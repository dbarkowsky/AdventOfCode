use crate::utils;
use regex::Regex;

const DAY: &str = "04";
const TEST: bool = true;

// How many are true triangles? Two smallest sides > Biggest side
pub fn part_one() {
    let input: Vec<Instruction> = parse_input();
    println!("{:#?}", input);
}

// Same, but have to assemble triangles vertically from input instead
pub fn part_two() {

}

#[derive(Debug)]
struct Instruction {
  id: i32,
  checksum: String,
  hash: String,
}

fn parse_input() -> Vec<Instruction>{
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    return input_to_instruction(input);
}

fn input_to_instruction(input: Vec<String>) -> Vec<Instruction> {
  let pattern: Regex = Regex::new(r"(.*)([\d]{3})\[([a-z]+)\]").unwrap();
  return input.iter().map(|plain_input| {
    let temp: Vec<Instruction> = pattern.captures_iter(&plain_input).filter_map(|mat| {
      let groups = (mat.get(1), mat.get(2), mat.get(3));
      match groups {
        (Some(hash),Some(id), Some(checksum)) =>{ 
          let parsed_id = id.as_str().parse::<i32>().unwrap_or_default();
          Some(Instruction {
          id: parsed_id,
          checksum: checksum.as_str().to_string(),
          hash: hash.as_str().replace("-", "").to_string(),
        })},
        _ => None
      }
    }).collect();
    return temp;
  }).flatten().collect();
}
