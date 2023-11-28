import Foundation

class Day08 : Day {

  var listOfTuples: Array<(raw: String, actual: String)> = []

  override init(fileName: String){
    super.init(fileName: fileName)
    // Break up input
    listOfTuples = input.split(separator: "\r\n").map{ (raw: #"\#(String($0))"#, actual: String($0)) }
  }

  func RemoveOuterQuotes(_ word: String) -> String {
    var transformedWord = word
    // Remove outer quotes if present
        if transformedWord.hasPrefix("\"") && transformedWord.hasSuffix("\"") {
            transformedWord.removeFirst()
            transformedWord.removeLast()
        }

    return transformedWord
  }

  // Removes certain substrings from words. Took me forever to realize that I needed to replace them with something arbitrary.
  // Otherwise, they would affect the following steps.
  func Sanitize(word: String) -> String {
    var transformedWord = RemoveOuterQuotes(word)
    while let r = transformedWord.range(of: "\\\\")
    {
        transformedWord.replaceSubrange(r, with: "7")
    }
    while let r = transformedWord.range(of: "\\\"")
    {
        transformedWord.replaceSubrange(r, with: "8")
    }
    while let r = transformedWord.range(of: "\\x")
    {
        transformedWord.replaceSubrange(r, with: "")
        transformedWord.replaceSubrange(r, with: "9")
    }

    return transformedWord
  }

  func Part01() -> Int { 
    var rawCount: Int = 0
    var saniCount: Int = 0
    for tuple in listOfTuples {
      let sanitizedWord = Sanitize(word: tuple.actual);
      // print("\(tuple.raw) \(GetCodeLength(rawWord: tuple.raw)) \(sanitizedWord) \(sanitizedWord.count)")
      rawCount += tuple.raw.count
      saniCount += sanitizedWord.count
    }

    return rawCount - saniCount
  }

  func Part02() -> Int {
    return -1
  }
}
