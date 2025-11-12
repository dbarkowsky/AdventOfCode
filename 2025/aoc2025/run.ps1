param(
    [Parameter(Mandatory=$true)][string]$Day,
    [string]$Test
)

# Validate day is a number
if ($Day -notmatch '^\d+$') {
    Write-Host "Usage: .\run.ps1 <day> [test]"
    exit 1
}

# Compile
mvn clean package assembly:single

# Run
java -jar ".\target\aoc2025.jar" $Day $Test
