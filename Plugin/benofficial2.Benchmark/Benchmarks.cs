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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;

namespace benofficial2.Benchmark
{
    [MemoryDiagnoser]
    public class Benchmarks
    {
        private int[] data;

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random(42);
            data = System.Linq.Enumerable.Range(0, 1_000).Select(_ => rand.Next(0, 100)).ToArray();
        }

        [Benchmark(Baseline = true)]
        public int SumForLoop()
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i++)
                sum += data[i];
            return sum;
        }

        [Benchmark]
        public int SumForeach()
        {
            int sum = 0;
            foreach (var v in data)
                sum += v;
            return sum;
        }

        [Benchmark]
        public int SumLinq()
        {
            return System.Linq.Enumerable.Sum(data);
        }
    }
}
