use crate::utils;

const DAY: &str = "03";
const TEST: bool = false;

pub fn part_one() {
  let triangles = parse_input();
  let mut goodTriangles = 0;
  for triangle in triangles {
    if (triangle[0] + triangle[1] > triangle[2]){
      goodTriangles += 1;
    }
  }
  println!("{}", goodTriangles);
}

pub fn part_two() {
}

fn parse_input() -> Vec<Vec<i32>> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    let mut numberInput: Vec<Vec<i32>> = Vec::new();
    for row in input {
      // This is some real confusing logic for how to filter. Pain with referencing and dereferencing.
      let mut numbers: Vec<i32> = row.split(" ").filter(|item: &&str| *item != "").map(|s: &str| s.parse::<i32>().unwrap()).collect();
      numbers.sort();
      numberInput.push(numbers);
    }
    return numberInput;
}
