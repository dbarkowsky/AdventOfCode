use crate::utils;
use md5;

const DAY: &str = "05";
const TEST: bool = false;

// When hash starts with 00000, the next character goes on the password
pub fn part_one() {
    let input: Vec<String> = parse_input();
    let key: &String = &input[0]; // Because there's only one line
    const PASSWORD_LENGTH: i8 = 8;
    let mut password: String = String::new(); // We will append to this
    let mut index: u64 = 0;
    // Until it meets the length requirement
    while (password.len() as i8) < PASSWORD_LENGTH {
        // combine key with index
        let combo: String = format!("{}{}", key, index);
        let hash: md5::Digest = md5::compute(combo); // Get MD5 hash
        let hash_str: String = format!("{:x}", hash); // Convert the hash to a hexadecimal string

        // Check for starting zeroes, then push
        if &hash_str[0..5] == "00000" {
            password.push(hash_str.chars().nth(5).unwrap());
        }
        index += 1;
    }

    println!("{}", password);
}

// This time, 6th character is the position, 7th is the password value
pub fn part_two() {
    let input: Vec<String> = parse_input();
    let key = &input[0];
    const PASSWORD_LENGTH: u32 = 8;
    // We use this vector to define the known max size and prepopulate
    let mut password: Vec<Option<char>> = vec![None, None, None, None, None, None, None, None];
    let mut index: u64 = 0;
    // Until there are no more "None" values in password
    while password
        .iter()
        .any(|letter: &Option<char>| letter.is_none())
    {
        // combine key with index
        let combo: String = format!("{}{}", key, index);
        let hash: md5::Digest = md5::compute(combo);
        let hash_str: String = format!("{:x}", hash); // Convert the hash to a hexadecimal string

        if &hash_str[0..5] == "00000" {
            // Get position, and if it could be a number, use that as the position in the password vector
            let position: char = hash_str.chars().nth(5).unwrap();
            if position.is_digit(10) {
                let digit: u32 = position.to_digit(10).unwrap();
                // It has to be in bounds and an empty space
                if digit <= PASSWORD_LENGTH - 1 && password[digit as usize] == None {
                    let character: char = hash_str.chars().nth(6).unwrap();
                    // Assign that element to this character
                    password[digit as usize] = Some(character);
                }
            }
        }
        index += 1;
    }
    let password_as_string: String = password
        .iter()
        .map(|option| option.unwrap())
        .collect::<String>();
    println!("{}", password_as_string);
}

fn parse_input() -> Vec<String> {
    let test_suffix = if TEST { ".test" } else { "" };
    let file_path: String = format!("./input/day{day}{test}.txt", day = DAY, test = test_suffix);
    let input: Vec<String> = utils::file_reader::file_to_line_array(&file_path);
    return input;
}
