import CryptoKit

class Day04 : Day {

  var secretKey: String = ""

  override init(fileName: String){
    super.init(fileName: fileName)
    secretKey = String(input.split(separator: "\r\n").first!)
  }

  func Part01() -> Int {
    var appendedNumber: Int = 0
    var key = "abcdef609043"//"\(secretKey)\(appendedNumber)"
    // var hashResult: MD5Digest = key.utf8.md5
    print(Insecure.MD5.hash(data: key))

    // while (!(hashResult).hasPrefix("00000")) {
      
    //   hashResult = key.utf8.md5 //Insecure.MD5.hash(data: "\(secretKey)\(appendedNumber)")
    // }

    return appendedNumber
  }

  func Part02() -> Int {
    return -1
  }
}
