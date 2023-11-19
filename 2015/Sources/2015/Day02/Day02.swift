class Day02 : Day {

  var listOfBoxes: Array<Substring> = []
  var listOfBoxTuples: Array<(Int, Int, Int)> = []

  override init(fileName: String){
    super.init(fileName: fileName)
    // Break up input
    listOfBoxes = input.split(separator: "\r\n")
    do{
      try listOfBoxTuples = listOfBoxes.map(DimensionsToTuple) // from "1x1x1" to (1,1,1)
    } catch {
      print("Failed to convert dimensions")
    }
  }

  func DimensionsToTuple(_ dimensions: Substring) throws -> (Int, Int, Int) {
    let dimensionList = dimensions.split(separator: "x").map{ Int($0) }
    return (dimensionList[0]!, dimensionList[1]!, dimensionList[2]!)
  }

  func Part01() -> Int {
    var totalFeetOfPaper = 0

    for gift in listOfBoxTuples {
      let length = gift.0
      let width = gift.1
      let height = gift.2

      let surfaceArea = (2 * length * width) + (2 * width * height) + (2 * height * length)
      let slack = min(length * width, width * height, height * length)

      totalFeetOfPaper += surfaceArea + slack
    }
    return totalFeetOfPaper
  }

  func Part02() -> Int {
    var totalFeetOfRibbon = 0

    for gift in listOfBoxTuples {
      let length = gift.0
      let width = gift.1
      let height = gift.2

      let twoShortestSides = [length, width, height].sorted().dropLast() // Remove biggest side
      let shortestDistance = (2 * twoShortestSides.first!) + (2 * twoShortestSides.last!) 
      let ribbonForBow = length * width * height
      
      totalFeetOfRibbon += shortestDistance + ribbonForBow
    }
    return totalFeetOfRibbon
  }
}
