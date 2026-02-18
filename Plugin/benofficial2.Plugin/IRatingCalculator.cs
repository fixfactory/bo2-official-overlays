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
        public float IRatingChange { get; set; }
        public uint NewIRating { get; set; }

        public RaceResult(T driver, uint finishRank, uint startIRating, bool started, float iRatingChange = 0f, uint newIRating = 0)
        {
            Driver = driver;
            FinishRank = finishRank;
            StartIRating = startIRating;
            Started = started;
            IRatingChange = iRatingChange;
            NewIRating = newIRating == 0 ? startIRating : newIRating;
        }
    }

    public static class IRatingCalculator
    {
        private const int MaxPreallocatedSize = 64;
        private static readonly float[] s_preallocatedExpectedScores = new float[MaxPreallocatedSize];
        private static readonly float[] s_preallocatedFudgeFactors = new float[MaxPreallocatedSize];
        private static readonly float[] s_preallocatedChanges = new float[MaxPreallocatedSize];

        public static void Calculate<T>(List<RaceResult<T>> raceResults)
        {
            int numRegistrations = raceResults.Count;
            if (numRegistrations == 0)
                return;

            float br1 = 1600f / (float)Math.Log(2);

            int numStarters = 0;
            for (int i = 0; i < numRegistrations; i++)
            {
                if (raceResults[i].Started)
                    numStarters++;
            }
            int numNonStarters = numRegistrations - numStarters;

            float[] expectedScores;
            float[] fudgeFactors;
            float[] changes;

            if (numRegistrations <= MaxPreallocatedSize)
            {
                expectedScores = s_preallocatedExpectedScores;
                fudgeFactors = s_preallocatedFudgeFactors;
                changes = s_preallocatedChanges;

                Array.Clear(expectedScores, 0, numRegistrations);
                Array.Clear(fudgeFactors, 0, numRegistrations);
                Array.Clear(changes, 0, numRegistrations);
            }
            else
            {
                expectedScores = new float[numRegistrations];
                fudgeFactors = new float[numRegistrations];
                changes = new float[numRegistrations];
            }

            for (int i = 0; i < numRegistrations; i++)
            {
                float aRating = raceResults[i].StartIRating;
                float score = -0.5f;

                for (int j = 0; j < numRegistrations; j++)
                {
                    float bRating = raceResults[j].StartIRating;
                    score += Chance(aRating, bRating, br1);
                }

                expectedScores[i] = score;
            }

            float x = numRegistrations - numNonStarters / 2f;
            for (int i = 0; i < numRegistrations; i++)
            {
                if (raceResults[i].Started)
                {
                    fudgeFactors[i] = (x / 2f - raceResults[i].FinishRank) / 100f;
                }
            }

            float sumChangesStarters = 0f;
            for (int i = 0; i < numRegistrations; i++)
            {
                if (raceResults[i].Started)
                {
                    float change = ((numRegistrations - raceResults[i].FinishRank - expectedScores[i] - fudgeFactors[i]) * 200f) / numStarters;
                    changes[i] = change;
                    sumChangesStarters += change;
                }
            }

            if (numNonStarters > 0)
            {
                float sumExpectedNonStarters = 0f;
                for (int i = 0; i < numRegistrations; i++)
                {
                    if (!raceResults[i].Started)
                    {
                        sumExpectedNonStarters += expectedScores[i];
                    }
                }

                float avgExpectedNonStarters = sumExpectedNonStarters / numNonStarters;
                float changeMultiplier = -sumChangesStarters / numNonStarters / avgExpectedNonStarters;

                for (int i = 0; i < numRegistrations; i++)
                {
                    if (!raceResults[i].Started)
                    {
                        changes[i] = changeMultiplier * expectedScores[i];
                    }
                }
            }

            for (int i = 0; i < numRegistrations; i++)
            {
                raceResults[i].IRatingChange = changes[i];
                raceResults[i].NewIRating = (uint)Math.Round(raceResults[i].StartIRating + changes[i]);
            }
        }

        private static float Chance(float a, float b, float factor)
        {
            if (a <= 0 || b <= 0)
                return 0.5f;

            float expA = (float)Math.Exp(-a / factor);
            float expB = (float)Math.Exp(-b / factor);
            return (1 - expA) * expB / ((1 - expB) * expA + (1 - expA) * expB);
        }
    }
}
