mod utils;

fn main() {
    let input = utils::file_reader::file_to_line_array("./test.txt");
    for line in input {
      println!("{}", line);
    }
}
