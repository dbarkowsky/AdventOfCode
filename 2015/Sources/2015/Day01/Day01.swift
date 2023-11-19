class Day01 : Day {
  var listOfCommands: Array<Substring> = []
  override init(fileName: String){
    super.init(fileName: fileName)
    // Break up input
    listOfCommands = input.split(separator: "")
    listOfCommands.removeLast() // remove /r/n from end
  }
  func Part01() -> Int {
    var floor = 0
    for command in listOfCommands {
      if command == "(" {
        floor += 1
      } else {
        floor -= 1
      }
    }

    return floor
  }

  func Part02() -> Int {
    var floor = 0
    var count = 0

    for command in listOfCommands {
      count += 1
      if command == "(" {
        floor += 1
      } else {
        floor -= 1
      }

      if floor < 0 {
        return count
      }
    }

    return 0
  }
}
