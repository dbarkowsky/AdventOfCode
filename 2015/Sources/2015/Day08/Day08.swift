import Foundation

class Day08 : Day {

  var listOfTuples: Array<(raw: String, actual: String)> = []
  var partTwoAnswer: Int = 0

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

  // Did this one a little hacky-er
  // We're just going to replace substrings if found.
  // Don't care if the result is the actual word wanted, just needs the correct length
  func Dirty(word: String) -> String {
    var transformedWord = word
    print(transformedWord)
    while let r = transformedWord.range(of: "\"")
    {
        transformedWord.replaceSubrange(r, with: "77")
    }

    while let r = transformedWord.range(of: "\\")
    {
        transformedWord.replaceSubrange(r, with: "88")
    }

    // Append two random characters to spoof adding surrounding quotes
    transformedWord.append("99")
    return transformedWord
  }

  func Part01() -> Int { 
    var rawCount: Int = 0
    var saniCount: Int = 0
    
    // Pre-work for part 2
    var dirtyCount: Int = 0

    // Loop through tuples
    for tuple in listOfTuples {
      let sanitizedWord = Sanitize(word: tuple.actual); // Get cleaned word
      // print("\(tuple.raw) \(GetCodeLength(rawWord: tuple.raw)) \(sanitizedWord) \(sanitizedWord.count)")
      // Track lengths of words
      rawCount += tuple.raw.count
      saniCount += sanitizedWord.count

      // Pre-work for part 2
      let dirtyWord = Dirty(word: tuple.actual)
      dirtyCount += dirtyWord.count
    }

    // Store value for part 2
    partTwoAnswer = dirtyCount - rawCount;

    return rawCount - saniCount
  }

  func Part02() -> Int {
    return partTwoAnswer
  }
}
