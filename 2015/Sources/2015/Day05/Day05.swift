class Day05 : Day {

  var words: Array<String> = []

  override init(fileName: String){
    super.init(fileName: fileName)
    words = input.split(separator: "\r\n").map{ String($0) }
    words.removeLast() // removing \r\n
  }

  func LetterIsVowel(_ letter: Character) -> Bool {
    if (letter == "a" || letter == "e" || letter == "o" || letter == "i" || letter == "u"){
      return true
    }
    return false
  }

  func ContainsThreeVowels(_ word: String) -> Bool {
    var vowelCount = 0
    // Go through each letter, check if it's a vowel    
    word.forEach { letter in
      if LetterIsVowel(letter){
        vowelCount += 1
      }
    }
    if (vowelCount >= 3){
      return true
    }
    return false
  }

  // Looking for instances of xx patterns
  func ContainsDoubleLetter(_ word: String) -> Bool {
    // Go through word, compare two letters at a time
    // Start from index 1, go to end
    for i in 1..<(word.count) {
      // Dear Swift, this is a horrible way to substring
      let letter1 = word[word.index(word.startIndex, offsetBy: i - 1)]
      let letter2 = word[word.index(word.startIndex, offsetBy: i)]
      if letter1 == letter2 {
        return true
      }
    }
    return false
  }

  // Looking for instances of xyx patterns
  func ContainsLetterSandwich(_ word: String) -> Bool {
    // Go through word, compare two letters at a time
    // Start from index 1, go to end
    for i in 2..<(word.count) {
      // Dear Swift, this is a horrible way to substring
      let letter1 = word[word.index(word.startIndex, offsetBy: i - 2)]
      let letter2 = word[word.index(word.startIndex, offsetBy: i)]
      if letter1 == letter2 {
        return true
      }
    }
    return false
  }

  // Looking for how many instances where any two letters repeat twice..., but no sharing letters
  func HasDoubles(_ word: String) -> Bool {
    var i = 1
    // Don't need to check the last two letters. Should already have been matched if there is one.
    while (i < word.count - 2) {
      let letter1 = word[word.index(word.startIndex, offsetBy: i - 1)]
      let letter2 = word[word.index(word.startIndex, offsetBy: i)]
      let pair = "\(letter1)\(letter2)"

      let remainderOfWord = word.suffix(word.count - (i + 1)) // Word length (16) - (how far traversed + go to next index)
      if remainderOfWord.contains(pair) {
        return true
      }
      i += 1
    }
    return false
  }

  // Check if word contains any of the "bad" letter combos
  func ContainsBadLetters(_ word: String) -> Bool {
    if (
      word.contains("ab") ||
      word.contains("cd") ||
      word.contains("pq") ||
      word.contains("xy")
    ) {
      return true
    }
    return false
  }

  func Part01() -> Int {
    var niceWordCount = 0
    for word in words {
      if (
        !ContainsBadLetters(word) &&
        ContainsDoubleLetter(word) &&
        ContainsThreeVowels(word)
      ){
        niceWordCount += 1
      }
    }
    return niceWordCount
  }

  func Part02() -> Int {
    var niceWordCount = 0
    for word in words {
      if (
        HasDoubles(word) &&
        ContainsLetterSandwich(word) 
      ){
        niceWordCount += 1
      }
    }
    return niceWordCount
  }
}
