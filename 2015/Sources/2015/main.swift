import Foundation

// Thanks @HarshilShah for this idea.
typealias CurrentDay = Day06

let startTime = Date()

print("\n--- Input---")
let day = CurrentDay(fileName: "Day06.txt")
print("Part 1: \(day.Part01())")
print("Part 2: \(day.Part02())")

let endTime = Date()

print("Run time (seconds): \(startTime.distance(to: endTime))")
