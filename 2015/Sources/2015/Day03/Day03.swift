class Day03 : Day {

  var listOfInstructions: Array<Substring> = []

  override init(fileName: String){
    super.init(fileName: fileName)
    // Break up input
    listOfInstructions = input.split(separator: "")
    listOfInstructions.removeLast() // remove \r\n
  }

  // Uses references to update original x and y passed in
  func UpdateXY(instruction: Substring, x: inout Int, y: inout Int){
    switch (instruction) {
        case "<":
          x -= 1
        case ">":
          x += 1
        case "v":
          y -= 1
        case "^":
          y += 1
        default:
          print("Bad direction \(instruction)")
      }
  }

  func Part01() -> Int {
    var locationsVisited: [String:Int] = [:]
    var x = 0
    var y = 0

    // Add starting location
    locationsVisited["\(x),\(y)", default: 0] += 1
    // For each instruction, increment x or y, add to locationsVisited
    for instruction in listOfInstructions {
      UpdateXY(instruction: instruction, x: &x, y: &y)
      locationsVisited["\(x),\(y)", default: 0] += 1
    }
    // Count number of unique locations
    return locationsVisited.count
  }

  func Part02() -> Int {
    var locationsVisited: [String:Int] = [:]
    var x = 0
    var y = 0
    var x2 = 0
    var y2 = 0

    // Add starting location
    locationsVisited["\(x),\(y)", default: 0] += 1
    // For each instruction, increment x or y, add to locationsVisited
    // Now have to go back and forth between Santa and Robo-Santa
    listOfInstructions.enumerated().forEach {(index, instruction) in 
      // instruction is type Dictionary<String, Int>.Element here
      if (index % 2 == 0){
        UpdateXY(instruction: instruction, x: &x, y: &y)
        locationsVisited["\(x),\(y)", default: 0] += 1
      } else {
        UpdateXY(instruction: instruction, x: &x2, y: &y2)
        locationsVisited["\(x2),\(y2)", default: 0] += 1
      }
    }

    // Count number of unique locations
    return locationsVisited.count
  }
}
