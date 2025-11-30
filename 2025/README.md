# AoC 2025

Building this year in Java.

Using Maven as the build system.

## Long Way

To recompile the code, run this from the `2025` folder:

`mvn clean package assembly:single`

To then run that code, use this command:

`java -jar .\aoc2025\target\aoc2025.jar <day number, no leading 0> ["test" if using test data]`

For example, for Day 1 and the test data, you'd use:

`java -jar .\aoc2025\target\aoc2025.jar 1 test`

## Quick Way

From the `aoc2025` folder, run one of the `run.ps1` or `run.sh` scripts provided.

You still add the arguements like this: `run.ps1 1 test`, but it handles the build for you.

The config for debugging also exists if you use VS Code, so F5 should also run the application, although you'll have to change the args in the `launch.json` file.
