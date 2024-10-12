use std::fs;

pub fn file_to_line_array(file_path: &str) -> Vec<String> {
  let response = fs::read_to_string(file_path);
    let text_value = response.expect("file read");
    let input_array = text_value.lines().map(String::from).collect();
    return input_array;
}
