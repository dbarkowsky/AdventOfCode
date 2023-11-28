import Foundation

class Day08 : Day {

  var listOfTuples: Array<(raw: Int, actual: Int)> = []

  override init(fileName: String){
    super.init(fileName: fileName)
    // Break up input
    listOfTuples = input.split(separator: "\r\n").map{ (raw: #"\#(String($0))"#.count, actual: SubtractLengths(String($0))) }
    print(listOfTuples)
  }

  func SubtractLengths(_ word: String) -> Int{
    var toSubtract = 2 // For outer quotes
    print(word)
    // For hex
    var pattern = "[^\\\\]\\\\x[0-9a-f]{2}"
    var regex = try! NSRegularExpression(pattern: pattern, options: .caseInsensitive)
    let nsString = word as NSString
    var matches = regex.matches(in: word, options: [], range: NSMakeRange(0, nsString.length))
    print(matches.count)
    toSubtract += (matches.count * 3) // Lose 3 characters for this conversion
    // For \\
    pattern = "\\\\\\\\"
    regex = try! NSRegularExpression(pattern: pattern, options: .caseInsensitive)
    matches = regex.matches(in: word, options: [], range: NSMakeRange(0, nsString.length))
        print(matches.count)

    toSubtract += matches.count
    // For \"
    pattern = "[^\\\\]\\\\\""
    regex = try! NSRegularExpression(pattern: pattern, options: .caseInsensitive)
    matches = regex.matches(in: word, options: [], range: NSMakeRange(0, nsString.length))
        print(matches.count)

    toSubtract += matches.count

    return word.count - toSubtract
  }

  func GetCodeLength(rawWord: String) -> Int {
    return rawWord.count
  }

  func GetMemoryLength(word: String) -> Int {
    return word.length
  }

  func ConvertHexChars(_ word: String) -> String { 
    let pattern = "\\\\x([0-9A-Fa-f]{2})"
    if let regex = try? NSRegularExpression(pattern: pattern, options: []) {
      // Find matches in the input string
      let matches = regex.matches(in: word, options: [], range: NSRange(location: 0, length: word.utf16.count))
      
      // Iterate over matches and replace them
      var resultString = word
      for match in matches.reversed() {
        let range = Range(match.range, in: resultString)!
        let hexString = (resultString as NSString).substring(with: match.range(at: 1))
        if let unicodeScalarCode = UInt32(hexString, radix: 16), let unicodeScalar = UnicodeScalar(unicodeScalarCode) {
            let replacement = String(unicodeScalar)
            resultString = resultString.replacingCharacters(in: range, with: replacement)
        }
      }
      return resultString
    } else {
      print("Invalid regular expression pattern.")
      return ""
    }
  } 

  func RemoveOuterQuotes(_ word: String) -> String {
    var transformedWord = word
    // if (transformedWord.count > 2){
    //   let startIndex = transformedWord.index(after: transformedWord.startIndex)
    //   let endIndex = transformedWord.index(before: transformedWord.endIndex)
    //   transformedWord = String(transformedWord[startIndex..<endIndex])
    // } else if (transformedWord.count == 2) {
    //   return ""
    // }

    // Remove outer quotes if present
        if transformedWord.hasPrefix("\"") && transformedWord.hasSuffix("\"") {
            transformedWord.removeFirst()
            transformedWord.removeLast()
        }

    return transformedWord
  }

  func Sanitize(word: String) -> String {
    var transformedWord = word
      print("stage 1: \(transformedWord)")       
      transformedWord = ConvertHexChars(word)

      print("stage 2: \(transformedWord)")
             transformedWord = RemoveOuterQuotes(transformedWord)


      print("stage 3: \(transformedWord)")
          transformedWord = transformedWord.replacingOccurrences(of: #"\\"#, with: #"\"#)

      print("stage 4: \(transformedWord)")
          transformedWord = transformedWord.replacingOccurrences(of: #"\""#, with: #"\#""#)

      print("stage 5: \(transformedWord)")
    
    return transformedWord
  }

  func Part01() -> Int {
    var codeCount = 0
    var memoryCount = 0
    for tuple in listOfTuples {
      codeCount += tuple.raw
      memoryCount += tuple.actual
      // print("\(tuple.raw) \(GetCodeLength(rawWord: tuple.raw)) \(tuple.actual) \(GetMemoryLength(word: tuple.actual))")
    }

    return codeCount - memoryCount
  }

  func Part02() -> Int {
    return -1
  }
}
