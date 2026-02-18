/*
    benofficial2's Official Overlays
    Copyright (C) 2026 benofficial2

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using benofficial2.Plugin;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace benofficial2.Tests
{
    [TestClass]
    public sealed class IRatingCalculatorTests
    {
        [TestMethod]
        public void TestIRatingCalculation()
        {
            string testDataPath = @"..\..\..\..\..\Test Data\eventresult-82990708.json";
            
            var raceResults = LoadRaceResults(testDataPath);
            Assert.IsNotNull(raceResults);
            Assert.IsTrue(raceResults.Count > 0, "Race results should not be empty");

            // Clear calculation results before calculation
            foreach (var raceResult in raceResults)
            {
                raceResult.NewIRating = 0;
                raceResult.IRatingChange = 0;
            }

            var expectedResults = LoadRaceResults(testDataPath);
            Assert.IsNotNull(expectedResults);
            Assert.AreEqual(raceResults.Count, expectedResults.Count, "Expected results count should match race results count");

            IRatingCalculator.Calculate(raceResults);

            // Compare calculated results with expected results
            for (int i = 0; i < raceResults.Count; i++)
            {
                var calculated = raceResults[i];
                var expected = expectedResults.FirstOrDefault(e => e.Driver == calculated.Driver);

                Assert.IsNotNull(expected, $"Expected result not found for driver {calculated.Driver}");
                Assert.AreEqual(expected!.StartIRating, calculated.StartIRating,
                    $"Start iRating mismatch for {calculated.Driver}");
                Assert.AreEqual(expected.FinishRank, calculated.FinishRank,
                    $"Finish rank mismatch for {calculated.Driver}");
                Assert.AreEqual(expected.Started, calculated.Started,
                    $"Started status mismatch for {calculated.Driver}");

                // Allow for small floating point differences in iRating change
                Assert.AreEqual(expected.IRatingChange, calculated.IRatingChange, 1.0f,
                    $"iRating change mismatch for {calculated.Driver}: expected {expected.IRatingChange}, got {calculated.IRatingChange}");
                Assert.AreEqual(expected.NewIRating, calculated.NewIRating, 1u,
                    $"New iRating mismatch for {calculated.Driver}: expected {expected.NewIRating}, got {calculated.NewIRating}");
            }
        }       

        private static List<RaceResult<string>> LoadRaceResults(string filePath)
        {
            var results = new List<RaceResult<string>>();

            // Load the JSON file into a JObject
            var json = JObject.Parse(File.ReadAllText(filePath));

            var sessionResults = json["data"]?["session_results"] as JArray;
            if (sessionResults == null)
                return results;

            foreach (var session in sessionResults)
            {
                // Only race sessions (simsession_type == 6)
                if ((int?)session["simsession_type"] != 6)
                    continue;

                var drivers = session["results"] as JArray;
                if (drivers == null)
                    continue;

                foreach (var driver in drivers)
                {
                    string? name = (string?)driver["display_name"];
                    if (name == null) continue;

                    uint oldIRating = (uint)driver["oldi_rating"]!;
                    uint newIRating = (uint)driver["newi_rating"]!;
                    uint finishPosClass = (uint)driver["finish_position_in_class"]!;
                    uint lapsComplete = (uint)driver["laps_complete"]!;

                    bool started = lapsComplete > 0;

                    var result = new RaceResult<string>(
                        name,
                        finishPosClass + 1,
                        oldIRating,
                        started,
                        (float)newIRating - (float)oldIRating,
                        newIRating
                    );

                    results.Add(result);
                }
            }

            return results;
        }
    }
}
