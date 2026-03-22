#!/bin/bash
# Run tests with code coverage
dotnet test Vali-Tempo.Tests/Vali-Tempo.Tests.csproj \
  --settings Vali-Tempo.Tests/coverlet.runsettings \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage \
  /p:CoverletOutputFormat=cobertura \
  /p:CoverletOutput=./coverage/

echo ""
echo "Coverage report generated in ./coverage/"
echo "To generate HTML report: reportgenerator -reports:./coverage/*/coverage.cobertura.xml -targetdir:./coverage/html -reporttypes:Html"
