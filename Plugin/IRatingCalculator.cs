/*
    benofficial2's Official Overlays
    Copyright (C) 2025-2026 benofficial2

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;

namespace benofficial2.Plugin
{
    public class RaceResult<T>
    {
        public T Driver { get; set; }
        public uint FinishRank { get; set; }
        public uint StartIRating { get; set; }
        public bool Started { get; set; }

        public RaceResult(T driver, uint finishRank, uint startIRating, bool started)
        {
            Driver = driver;
            FinishRank = finishRank;
            StartIRating = startIRating;
            Started = started;
        }
    }

    public class CalculationResult<T>
    {
        public RaceResult<T> RaceResult { get; set; }
        public float IRatingChange { get; set; }
        public uint NewIRating { get; set; }
    }

    public static class IRatingCalculator
    {
        public static List<CalculationResult<T>> Calculate<T>(List<RaceResult<T>> raceResults)
        {
            float br1 = 1600f / (float)Math.Log(2);

            int numRegistrations = raceResults.Count;
            int numStarters = raceResults.Count(r => r.Started);
            int numNonStarters = numRegistrations - numStarters;

            var chances = raceResults.Select(a =>
            {
                float aRating = a.StartIRating;
                return raceResults.Select(b =>
                {
                    float bRating = b.StartIRating;
                    return Chance(aRating, bRating, br1);
                }).ToList();
            }).ToList();

            var expectedScores = chances.Select(ch => ch.Sum() - 0.5f).ToList();

            var fudgeFactors = raceResults.Select(r =>
            {
                if (!r.Started) return 0f;
                float x = numRegistrations - numNonStarters / 2f;
                return (x / 2f - r.FinishRank) / 100f;
            }).ToList();

            var changesStarters = raceResults.Zip(expectedScores, (r, s) => (r, s))
                .Zip(fudgeFactors, (rs, f) =>
                {
                    var (r, s) = rs;
                    if (!r.Started) return (float?)null;
                    return ((numRegistrations - r.FinishRank - s - f) * 200f) / numStarters;
                }).ToList();

            float sumChangesStarters = changesStarters.Where(c => c.HasValue).Sum(c => c.Value);

            var expectedNonStarters = raceResults.Zip(expectedScores, (r, s) => r.Started ? (float?)null : s).ToList();
            float sumExpectedNonStarters = expectedNonStarters.Where(x => x.HasValue).Sum(x => x.Value);

            var changesNonStarters = expectedNonStarters.Select(s =>
                s.HasValue ? -sumChangesStarters / numNonStarters * s.Value / (sumExpectedNonStarters / numNonStarters) : (float?)null
            ).ToList();

            var changes = changesStarters.Zip(changesNonStarters, (a, b) => a ?? b ?? throw new Exception("Invalid state")).ToList();

            return raceResults.Zip(changes, (r, c) => new CalculationResult<T>
            {
                RaceResult = r,
                IRatingChange = c,
                NewIRating = (uint)Math.Round(r.StartIRating + c)
            }).ToList();
        }

        private static float Chance(float a, float b, float factor)
        {
            if (a <= 0 || b <= 0)
                return 0.5f;

            float expA = (float)Math.Exp(-a / factor);
            float expB = (float)Math.Exp(-b / factor);
            return (1 - expA) * expB / ((1 - expB) * expA + (1 - expA) * expB);
        }

        public static List<RaceResult<string>> LoadRaceResults(string filePath)
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
                    string name = (string)driver["display_name"];
                    uint oldIRating = (uint)driver["oldi_rating"];
                    uint finishPosClass = (uint)driver["finish_position_in_class"];
                    uint lapsComplete = (uint)driver["laps_complete"];

                    bool started = lapsComplete > 0;

                    results.Add(new RaceResult<string>(
                        name,
                        finishPosClass + 1,
                        oldIRating,
                        started
                    ));
                }
            }

            return results;
        }

        public static void Test(string raceResultsJsonFile)
        {
            var raceResults = LoadRaceResults(raceResultsJsonFile);
            SimHub.Logging.Current.Info("Race Results:");
            foreach (var result in raceResults)
            {
                SimHub.Logging.Current.Info($"{result.Driver} | Start iRating: {result.StartIRating} | Finish Rank: {result.FinishRank} | Started: {result.Started}");
            }

            var results = IRatingCalculator.Calculate(raceResults);
            SimHub.Logging.Current.Info("Calculation Results:");
            foreach (var result in results)
            {
                SimHub.Logging.Current.Info($"{result.RaceResult.Driver} | Start iRating: {result.RaceResult.StartIRating} | Finish Rank: {result.RaceResult.FinishRank} | Started: {result.RaceResult.Started} | IRating Change: {result.IRatingChange} | New iRating: {result.NewIRating}");
            }
        }
    }
}
