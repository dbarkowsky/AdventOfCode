class Day07 : Day {
  // Breaking the commands into tuples for easier parsing later
  var commands: Array<String> = []
  var commandTuples: Array<(command:String, register:String)> = []

  override init(fileName: String){
    super.init(fileName: fileName)
    commands = input.split(separator: "\r\n").map{ String($0) }
    commandTuples = commands.map{ ParseInput($0) }
  }

  enum CommandType {
    case NUMBER
    case AND
    case OR
    case NOT
    case LSHIFT
    case RSHIFT
  }

  // Turn command lines into command tuples
  func ParseInput(_ command: String) -> (command:String, register:String) {
    let pair = command.split(separator: " -> ").map{ String($0) }
    return (command: pair[0], register: pair[1])
  }

  // Inspect command and decide on action
  func ParseCommand(_ command: String) -> CommandType {
    switch true {
      case command.contains("AND"):
        return CommandType.AND
      case command.contains("OR"):
        return CommandType.OR
      case command.contains("NOT"):
        return CommandType.NOT
      case command.contains("LSHIFT"):
        return CommandType.LSHIFT
      case command.contains("RSHIFT"):
        return CommandType.RSHIFT
      default:
        return CommandType.NUMBER
    }
  }

  // Convert Int to UInt16
  func IntToUInt16(_ num: Int) -> UInt16 {
    return UInt16(num)
  }

  // Convert UInt16 to Int
  func UInt16ToInt(_ num: UInt16) -> Int {
    return Int(num)
  }

  // Bitwise AND two UInt16
  func AND(_ num1: UInt16, _ num2: UInt16) -> UInt16 {
    return num1 & num2
  }

  // Bitwise OR two UInt16
  func OR(_ num1: UInt16, _ num2: UInt16) -> UInt16 {
    return num1 | num2
  }

  // Bitwise NOT a UInt16
  func NOT(_ num: UInt16) -> UInt16 {
    return ~num
  }

  // Bitwise Left Shift a UInt16
  func LSHIFT(_ num: UInt16, _ bits: Int) -> UInt16 {
    return num << bits
  }

  // Bitwise Right Shift a UInt16
  func RSHIFT(_ num: UInt16, _ bits: Int) -> UInt16 {
    return num >> bits
  }

  func Part01() -> Int {
    return -1
  }

  func Part02() -> Int {
    return -1
  }
}
