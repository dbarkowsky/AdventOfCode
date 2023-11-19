import Foundation
class Day {
  var input: String = "placeholder"

  init(fileName: String){
    print("Input: \(fileName)")
    let fileURL = "./Sources/2015/input/\(fileName)"

    do{
      input = try String(contentsOfFile: fileURL)
    }catch{
      print("Could not read file \(fileName)")
    }
  }
}
