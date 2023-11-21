class Day04 : Day {

  var secretKey: String = ""
  var part1Answer: Int = 0

  override init(fileName: String){
    super.init(fileName: fileName)
    secretKey = String(input.split(separator: "\r\n").first!)
  }

  func FindAppendedNumForPrefix(_ prefix: String, startAt: Int = 0) -> Int {
    var appendedNumber: Int = startAt
    var hashResult: String = MD5("\(secretKey)\(appendedNumber)")

    while (!(hashResult).hasPrefix(prefix)) {
      appendedNumber += 1
      hashResult = MD5("\(secretKey)\(appendedNumber)")
    }

    return appendedNumber
  }

  func Part01() -> Int {
    part1Answer = FindAppendedNumForPrefix("00000") // Store this for later to save time...
    return part1Answer
  }

  func Part02() -> Int {
    return FindAppendedNumForPrefix("000000", startAt: part1Answer)
  }
}
