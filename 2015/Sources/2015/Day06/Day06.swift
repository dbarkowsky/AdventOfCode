class Day06 : Day {
  // Breaking the commands into tuples for easier parsing later
  var commands: Array<String> = []
  var commandTuples: Array<(action:String, from:String, to:String)> = []
  // Instead of keeping a 1000x1000 2D array, let's store lights dictionary.
  var lights: [String:Int] = [:]

  override init(fileName: String){
    super.init(fileName: fileName)
    commands = input.split(separator: "\r\n").map{ String($0) }
    commandTuples = commands.map{ ParseCommands($0) }
  }

  // Turn command lines into command tuples
  func ParseCommands(_ command: String) -> (action:String, from:String, to:String) {
    let splitCommand = command.split(separator: " ").map{ String($0) }
    
    // Use number of elements to determine mapping
    if splitCommand.count == 5 {
      return (action: splitCommand[1], from: splitCommand[2], to: splitCommand[4])
    } else {
      return (action: splitCommand[0], from: splitCommand[1], to: splitCommand[3])
    }
  }

  // Turn a light off
  func TurnOff(_ coordinatePair: String) {
    lights[coordinatePair] = 0
  }

  // Turn a light on
  func TurnOn(_ coordinatePair: String) {
    lights[coordinatePair] = 1
  }

  // Toggle a light on or off
  func Toggle(_ coordinatePair: String) {
    if (lights[coordinatePair] == 1){
      TurnOff(coordinatePair)
    } else {
      TurnOn(coordinatePair)
    }
  }

  // Get total amount of lights that are on
  func GetLightSum() -> Int {
    var lightSum = 0
    for (_, value) in lights {
      lightSum += value
    }
    return lightSum
  }

  // Adjusts brightness by amount
  func AdjustBrightness(_ coordinatePair: String, amount: Int) {
    if (lights[coordinatePair] != nil) {
      // Already in dict, just adjust
      if (lights[coordinatePair]! + amount >= 0){
        lights[coordinatePair]! += amount
      }
    } else {
      // Doesn't exist in dict yet, only assign if not negative
      if (amount >= 0) {
        lights[coordinatePair] = amount
      }
    }
    
  }

  // Iterate through all lights from "from" to "to" locations in a commandTuple
  // Imaging a square of lights. "from" is the bottom left corner, "to" is the top right
  // This function should touch all lights in the "square"
  func DoCommand(_ command: (action:String, from:String, to:String)) throws {
    // A basic custom error
    enum ConversionError: Error {
      case notAnIntError(String)
    }
    
    // Making sure that they evaluate as Ints, else throw an error (also makes them as variables for later)
    guard let startingX = Int(command.from.split(separator: ",").first ?? ""),
      let startingY = Int(command.from.split(separator: ",").last ?? ""),
      let finalX = Int(command.to.split(separator: ",").first ?? ""),
      let finalY = Int(command.to.split(separator: ",").last ?? "") 
    else {
      throw ConversionError.notAnIntError("Something was not a number")
    }

    var currentX = startingX
    while currentX <= finalX {
      var currentY = startingY
      while currentY <= finalY {
        switch(command.action){
          case "on":
            TurnOn("\(currentX),\(currentY)")
          case "off":
            TurnOff("\(currentX),\(currentY)")
          case "toggle":
            Toggle("\(currentX),\(currentY)")
          default:
            print("Found bad action: \(command.action)")
        }
        currentY += 1
      }
      currentX += 1
    }
  }

  // Similar to the above function, but do different things on each command
  func DoCommandPartTwo(_ command: (action:String, from:String, to:String)) throws {
    // A basic custom error
    enum ConversionError: Error {
      case notAnIntError(String)
    }
    
    // Making sure that they evaluate as Ints, else throw an error (also makes them as variables for later)
    guard let startingX = Int(command.from.split(separator: ",").first ?? ""),
      let startingY = Int(command.from.split(separator: ",").last ?? ""),
      let finalX = Int(command.to.split(separator: ",").first ?? ""),
      let finalY = Int(command.to.split(separator: ",").last ?? "") 
    else {
      throw ConversionError.notAnIntError("Something was not a number")
    }

    var currentX = startingX
    while currentX <= finalX {
      var currentY = startingY
      while currentY <= finalY {
        switch(command.action){
          case "on":
            AdjustBrightness("\(currentX),\(currentY)", amount: 1)
          case "off":
            AdjustBrightness("\(currentX),\(currentY)", amount: -1)
          case "toggle":
            AdjustBrightness("\(currentX),\(currentY)", amount: 2)
          default:
            print("Found bad action: \(command.action)")
        }
        currentY += 1
      }
      currentX += 1
    }
  }

  func Part01() -> Int {
    for command in commandTuples {
      do {
        try DoCommand(command)
      } catch {
        print("Could not do command: \(command)")
      }
      
    }
    return GetLightSum()
  }

  func Part02() -> Int {
    // Reset dictionary
    lights = [:]
    for command in commandTuples {
      do {
        try DoCommandPartTwo(command)
      } catch {
        print("Could not do command: \(command)")
      }
      
    }
    return GetLightSum()
  }
}
