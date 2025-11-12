# !/bin/bash

# Get CLI arguments
DAY=$1
TEST=$2

# Check that day argument is provided and is a number
if [[ -z "$DAY" || ! "$DAY" =~ ^[0-9]+$ ]]; then
  echo "Usage: $0 <day> [test]"
  exit 1
fi

# Compile the project using Maven
mvn clean package assembly:single

java -jar ".\target\aoc2025.jar" $DAY $TEST
