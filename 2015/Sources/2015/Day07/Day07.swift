class Day07 : Day {
  // Breaking the commands into tuples for easier parsing later
  var commands: Array<String> = []
  // A list of tuples with the command and register key
  var registerTuples: Array<(command: String, register: String)> = []
  // Storing previously calculated values in this dict
  // Without this, there are so many repeated calculations, it takes forever!
  var previouslyCalculatedValues: [String: Int] = [:]

  override init(fileName: String){
    super.init(fileName: fileName)
    commands = input.split(separator: "\r\n").map{ String($0) }
    registerTuples = commands.map{ ParseInput($0) }
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
  func ParseInput(_ command: String) -> (command: String, register: String) {
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
  func LSHIFT(_ num: UInt16, _ distance: Int) -> UInt16 {
    return num << distance
  }

  // Bitwise Right Shift a UInt16
  func RSHIFT(_ num: UInt16, _ distance: Int) -> UInt16 {
    return num >> distance
  }

  // Checking if value already is a number or needs to be calculated
  // Helps cut down repetitive code in GetValueOfRegister
  func GetValueHelper(command: String, index: Int) -> UInt16 {
    let params = command.split(separator: " ").map{ String($0) }
    // If not nil, it's a number
    if (Int(params[index]) != nil){
      return IntToUInt16(Int(params[index])!)
    } else {
      // Then it must be a letter/register
      return IntToUInt16(GetValueOfRegister(key: params[index]))
    }
  }

  // Gets the value of a specific register
  // Recursively looks for register values needed
  func GetValueOfRegister(key: String) -> Int {
    // Has it already been calculated? Just use that.
    if (previouslyCalculatedValues[key] != nil){
      return previouslyCalculatedValues[key]!
    }

    // Otherwise, find that entry in the registerTuples
    if let entry = registerTuples.first(where: { $0.register == key}){
      let commandType = ParseCommand(entry.command)
      var result: Int = 0
      // Do the correct command
      switch (commandType){
        case CommandType.AND:
          let input1 = GetValueHelper(command: entry.command, index: 0)
          let input2 = GetValueHelper(command: entry.command, index: 2)
          result = UInt16ToInt(AND(input1, input2))
        case CommandType.OR:
          let input1 = GetValueHelper(command: entry.command, index: 0)
          let input2 = GetValueHelper(command: entry.command, index: 2)
          result = UInt16ToInt(OR(input1, input2))
        case CommandType.NOT:
          let input = GetValueHelper(command: entry.command, index: 1)
          result = UInt16ToInt(NOT(input))
        case CommandType.LSHIFT:
          let params = entry.command.split(separator: " ").map{ String($0) }
          let input = GetValueHelper(command: entry.command, index: 0)
          let distance = Int(params[2])!
          result = UInt16ToInt(LSHIFT(input, distance))
        case CommandType.RSHIFT:
          let params = entry.command.split(separator: " ").map{ String($0) }
          let input = GetValueHelper(command: entry.command, index: 0)
          let distance = Int(params[2])!
          result = UInt16ToInt(RSHIFT(input, distance))
        case CommandType.NUMBER:
          let input = GetValueHelper(command: entry.command, index: 0)
          result = UInt16ToInt(input)
      }
      // Add the result to the previously calculated values and return it.
      previouslyCalculatedValues[key] = result
      return result
    } else {
      // Should never reach this unless you ask for a non-existent key
      print("That key doesn't exist...")
      return -1
    }
  }

  func Part01() -> Int {
    return GetValueOfRegister(key: "a")
  }

  func Part02() -> Int {
    // Take signal from wire a
    let aValue = GetValueOfRegister(key: "a")
    // Override wire b with that value (assume b exists)
    let entryIndex = registerTuples.firstIndex(where: { $0.register == "b"})!
    registerTuples[entryIndex] = (command: "\(aValue)", register: "b")
    // Reset the wires memory
    previouslyCalculatedValues = [:]
    // Get new value of a
    return GetValueOfRegister(key: "a")
  }
}
