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

using GameReaderCommon;
using Newtonsoft.Json.Linq;
using SimHub.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace benofficial2.Plugin
{
    public class CarModule : PluginModuleBase
    {
        private DateTime _lastUpdateTime = DateTime.MinValue;
        private TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);

        private RemoteJsonFile _carInfo = new RemoteJsonFile("https://raw.githubusercontent.com/fixfactory/bo2-official-overlays/main/Data/CarInfo.json");
        private RemoteJsonFile _carBrandInfo = new RemoteJsonFile("https://raw.githubusercontent.com/fixfactory/bo2-official-overlays/main/Data/CarBrandInfo.json");
        private RemoteJsonFile _carClassInfo = new RemoteJsonFile("https://raw.githubusercontent.com/fixfactory/bo2-official-overlays/main/Data/CarClassInfo.json");

        private string _lastCarId = string.Empty;

        // Key: Car ID, Value: Car Brand
        private Dictionary<string, string> _carBrands = new Dictionary<string, string>();

        private static readonly List<object[]> _brakeBiasPaths = new List<object[]>
        {
            new object[] { "CarSetup", "Chassis", "Front", "BrakeBias" },
            new object[] { "CarSetup", "Chassis", "Front", "FrontBrakeBias" },
            new object[] { "CarSetup", "DriveBrake", "BrakeSystemConfig", "BaseBrakeBias" },
            new object[] { "CarSetup", "Suspension", "Front", "BrakeBias" }
        };

        public Dictionary<int, string> TireCompounds = null;

        public string Brand { get; set; } = string.Empty;
        public bool IsGT3 { get; set; } = false;
        public bool IsGT4 { get; set; } = false;
        public bool IsGTE { get; set; } = false;
        public bool IsGTP { get; set; } = false;
        public bool HasPushToPass { get; set; } = false;
        public bool HasPushToPassCount { get; set; } = false;
        public bool HasPushToPassTimer { get; set; } = false;
        public bool HasPushToPassCooldown { get; set; } = false;
        public bool HasDrsDetection { get; set; } = false;
        public bool HasDrsCount { get; set; } = false;
        public bool HasErs { get; set; } = false;
        public bool HasBoost { get; set; } = false;
        public bool HasEnginePowerMode { get; set; } = false;
        public bool HasDeployMode { get; set; } = false;
        public bool HasDeployModeType1 { get; set; } = false;
        public bool HasDeployModeType2 { get; set; } = false;
        public bool HasLapDeployLimit { get; set; } = false;
        public bool HasThrottleShaping { get; set; } = false;
        public bool HasFuelMix { get; set; } = false;
        public bool HasARBModeP { get; set; } = false;
        public bool HasFrontARB { get; set; } = false;
        public bool HasExitDiff { get; set; } = false;
        public bool HasEntryDiffPreload { get; set; } = false;
        public bool HasTC2 { get; set; } = false;
        public bool HasRearARB { get; set; } = false;
        public bool HasEntryDiff { get; set; } = false;
        public bool HasTC { get; set; } = false;
        public bool HasTwoPartBrakeBias { get; set; } = false;
        public bool HasWeightJacker { get; set; } = false;
        public bool HasTwoPartPeakBrakeBias { get; set; } = false;
        public bool HasABS { get; set; } = false;
        public bool HasFineBrakeBias { get; set; } = false;
        public bool HasBrakeBiasMigration { get; set; } = false;
        public bool HasDryTireCompounds { get; set; } = false;
        public bool HasRefueling { get; set; } = true;
        public double TotalBrakeBias { get; set; } = 0.0;
        public double TotalPeakBrakeBias { get; set; } = 0.0;
        public override int UpdatePriority => 20;
        public override void Init(PluginManager pluginManager, benofficial2 plugin)
        {
            _carInfo.LoadAsync();
            _carBrandInfo.LoadAsync();
            _carClassInfo.LoadAsync();

            plugin.AttachDelegate(name: "Car.Brand", valueProvider: () => Brand);
            plugin.AttachDelegate(name: "Car.IsGT3", valueProvider: () => IsGT3);
            plugin.AttachDelegate(name: "Car.IsGT4", valueProvider: () => IsGT4);
            plugin.AttachDelegate(name: "Car.IsGTE", valueProvider: () => IsGTE);
            plugin.AttachDelegate(name: "Car.IsGTP", valueProvider: () => IsGTP);
            plugin.AttachDelegate(name: "Car.HasPushToPass", valueProvider: () => HasPushToPass);
            plugin.AttachDelegate(name: "Car.HasPushToPassCount", valueProvider: () => HasPushToPassCount);
            plugin.AttachDelegate(name: "Car.HasPushToPassTimer", valueProvider: () => HasPushToPassTimer);
            plugin.AttachDelegate(name: "Car.HasPushToPassCooldown", valueProvider: () => HasPushToPassCooldown);
            plugin.AttachDelegate(name: "Car.HasDrsDetection", valueProvider: () => HasDrsDetection);
            plugin.AttachDelegate(name: "Car.HasDrsCount", valueProvider: () => HasDrsCount);
            plugin.AttachDelegate(name: "Car.HasErs", valueProvider: () => HasErs);
            plugin.AttachDelegate(name: "Car.HasBoost", valueProvider: () => HasBoost);
            plugin.AttachDelegate(name: "Car.HasEnginePowerMode", valueProvider: () => HasEnginePowerMode);
            plugin.AttachDelegate(name: "Car.HasDeployMode", valueProvider: () => HasDeployMode);
            plugin.AttachDelegate(name: "Car.HasDeployModeType1", valueProvider: () => HasDeployModeType1);
            plugin.AttachDelegate(name: "Car.HasDeployModeType2", valueProvider: () => HasDeployModeType2);
            plugin.AttachDelegate(name: "Car.HasLapDeployLimit", valueProvider: () => HasLapDeployLimit);
            plugin.AttachDelegate(name: "Car.HasThrottleShaping", valueProvider: () => HasThrottleShaping);
            plugin.AttachDelegate(name: "Car.HasFuelMix", valueProvider: () => HasFuelMix);
            plugin.AttachDelegate(name: "Car.HasARBModeP", valueProvider: () => HasARBModeP);
            plugin.AttachDelegate(name: "Car.HasFrontARB", valueProvider: () => HasFrontARB);
            plugin.AttachDelegate(name: "Car.HasExitDiff", valueProvider: () => HasExitDiff);
            plugin.AttachDelegate(name: "Car.HasEntryDiffPreload", valueProvider: () => HasEntryDiffPreload);
            plugin.AttachDelegate(name: "Car.HasTC2", valueProvider: () => HasTC2);
            plugin.AttachDelegate(name: "Car.HasRearARB", valueProvider: () => HasRearARB);
            plugin.AttachDelegate(name: "Car.HasEntryDiff", valueProvider: () => HasEntryDiff);
            plugin.AttachDelegate(name: "Car.HasTC", valueProvider: () => HasTC);
            plugin.AttachDelegate(name: "Car.HasTwoPartBrakeBias", valueProvider: () => HasTwoPartBrakeBias);
            plugin.AttachDelegate(name: "Car.HasWeightJacker", valueProvider: () => HasWeightJacker);
            plugin.AttachDelegate(name: "Car.HasTwoPartPeakBrakeBias", valueProvider: () => HasTwoPartPeakBrakeBias);
            plugin.AttachDelegate(name: "Car.HasABS", valueProvider: () => HasABS);
            plugin.AttachDelegate(name: "Car.HasFineBrakeBias", valueProvider: () => HasFineBrakeBias);
            plugin.AttachDelegate(name: "Car.HasBrakeBiasMigration", valueProvider: () => HasBrakeBiasMigration);
            plugin.AttachDelegate(name: "Car.HasDryTireCompounds", valueProvider: () => HasDryTireCompounds);
            plugin.AttachDelegate(name: "Car.HasRefueling", valueProvider: () => HasRefueling);
            plugin.AttachDelegate(name: "Car.TotalBrakeBias", valueProvider: () => TotalBrakeBias);
            plugin.AttachDelegate(name: "Car.TotalPeakBrakeBias", valueProvider: () => TotalPeakBrakeBias);
        }

        public override void DataUpdate(PluginManager pluginManager, benofficial2 plugin, ref GameData data)
        {
            if (data.FrameTime - _lastUpdateTime < _updateInterval) return;
            _lastUpdateTime = data.FrameTime;

            if (data.NewData.CarId != _lastCarId)
            {
                _lastCarId = data.NewData.CarId;
                UpdateFromJson(ref data);
                UpdateTireCompounds(ref data);

                // When the Auto Fill property is null, the car does not support refueling.
                if (RawDataHelper.TryGetTelemetryData<int>(ref data, out int dpFuelAutoFillEnabled, "dpFuelAutoFillEnabled"))
                    HasRefueling = true;
                else
                    HasRefueling = false;
            }

            UpdateBrakeBias(ref data);
        }

        public void UpdateTireCompounds(ref GameData data)
        {
            TireCompounds = new Dictionary<int, string>();

            if (!RawDataHelper.TryGetSessionData<List<object>>(ref data, out List<object> driverTires, "DriverInfo", "DriverTires"))
                return;

            if (driverTires == null)
                return;

            for (int i = 0; i < driverTires.Count; i++)
            {
                RawDataHelper.TryGetSessionData<string>(ref data, out string compoundType, "DriverInfo", "DriverTires", i, "TireCompoundType");
                RawDataHelper.TryGetSessionData<int>(ref data, out int tireIndex, "DriverInfo", "DriverTires", i, "TireIndex");

                if (compoundType.Length > 0)
                {
                    TireCompounds[tireIndex] = compoundType;
                }
            }
        }

        public void UpdateBrakeBias(ref GameData data)
        {
            if (data.NewData.CarId.Length == 0)
            {
                TotalBrakeBias = 0.0;
                TotalPeakBrakeBias = 0.0;
                return;
            }

            bool hasTelemetryBB = RawDataHelper.TryGetTelemetryData<double>(ref data, out double telemetryBB, "dcBrakeBias");
            RawDataHelper.TryGetTelemetryData<double>(ref data, out double telemetryFineBB, "dcBrakeBiasFine");
            RawDataHelper.TryGetTelemetryData<double>(ref data, out double telemetryPeakBB, "dcPeakBrakeBias");

            if (HasTwoPartBrakeBias || !hasTelemetryBB)
            {
                TryGetSetupBrakeBias(ref data, out double setupBB);
                TotalBrakeBias = Math.Round(telemetryBB, 2) + setupBB + telemetryFineBB;
            }
            else
            {
                TotalBrakeBias = Math.Round(telemetryBB, 1) + telemetryFineBB;
            }

            if (HasTwoPartPeakBrakeBias)
            {
                if (RawDataHelper.TryGetSessionData<string>(ref data, out string setupPeakBrakeBias, "CarSetup", "DriveBrake", "BrakeSystemConfig", "PeakBrakeBias"))
                {
                    TryParseBrakeBias(setupPeakBrakeBias, out double setupPeakBB);

                    // iRacing returns an integer where 0=-2%, 5=0%, 9=2%
                    // Tested with the McLaren MP430, but could be different for other cars.
                    double deltaPeakBB = (telemetryPeakBB - 5) * 0.5;
                    TotalPeakBrakeBias = setupPeakBB + deltaPeakBB;
                }
                else
                {
                    TotalPeakBrakeBias = 0.0;
                }
            }
            else
            {
                if (telemetryPeakBB > 0.0)
                {
                    // iRacing returns an integer where 1=0%, 2=1%, etc.
                    // Tested with the Mercedes W13, but could be different for other cars.
                    double deltaPeakBB = telemetryPeakBB - 1.0; 
                    TotalPeakBrakeBias = TotalBrakeBias + deltaPeakBB;
                }
                else
                {
                    TotalPeakBrakeBias = 0.0;
                }
            }            
        }

        public bool TryGetSetupBrakeBias(ref GameData data, out double brakeBias)
        {
            brakeBias = 0.0;

            if (!RawDataHelper.TryGetFirstSessionData<string>(ref data, out string setupBB, _brakeBiasPaths))
                return false;

            return TryParseBrakeBias(setupBB, out brakeBias);
        }

        public bool TryParseBrakeBias(string brakeBiasString, out double brakeBias)
        {
            brakeBias = 0.0;
            if (string.IsNullOrWhiteSpace(brakeBiasString)) 
                return false;

            // Matches the start of the string (^), skips optional whitespace (\s*), and captures a number that may contain a decimal point.
            // Examples: "52.0% (BBAL)", "57.0% front"
            var match = Regex.Match(brakeBiasString, @"^\s*(\d+(\.\d+)?)");
            if (!match.Success) 
                return false;

            return double.TryParse(match.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out brakeBias);
        }

        public void UpdateFromJson(ref GameData data)
        {
            JToken car = null;
            if (data.NewData.CarId.Length > 0 &&
                _carInfo.Json != null && 
                _carBrandInfo.Json != null)
            {
                car = _carInfo.Json[data.NewData.CarId];
            }
            
            if (car == null)
            {
                Brand = string.Empty;
                IsGT3 = false;
                IsGT4 = false;
                IsGTE = false;
                IsGTP = false;
                HasPushToPass = false;
                HasPushToPassCount = false;
                HasPushToPassTimer = false;
                HasPushToPassCooldown = false;
                HasDrsDetection = false;
                HasDrsCount = false;
                HasErs = false;
                HasBoost = false;
                HasEnginePowerMode = false;
                HasDeployMode = false;
                HasDeployModeType1 = false;
                HasDeployModeType2 = false;
                HasThrottleShaping = false;
                HasFuelMix = false;
                HasARBModeP = false;
                HasFrontARB = false;
                HasExitDiff = false;
                HasEntryDiffPreload = false;
                HasTC2 = false;
                HasRearARB = false;
                HasEntryDiff = false;
                HasTC = false;
                HasTwoPartBrakeBias = false;
                HasWeightJacker = false;
                HasTwoPartPeakBrakeBias = false;
                HasABS = false;
                HasFineBrakeBias = false;
                HasBrakeBiasMigration = false;
                HasDryTireCompounds = false;
                return;
            }

            Brand = GetCarBrand(data.NewData.CarId, data.NewData.CarModel);
            IsGT3 = car["isGT3"]?.Value<bool>() ?? false;
            IsGT4 = car["isGT4"]?.Value<bool>() ?? false;
            IsGTE = car["isGTE"]?.Value<bool>() ?? false;
            IsGTP = car["isGTP"]?.Value<bool>() ?? false;
            HasPushToPass = car["hasPushToPass"]?.Value<bool>() ?? false;
            HasPushToPassCount = car["hasPushToPassCount"]?.Value<bool>() ?? false;
            HasPushToPassTimer = car["hasPushToPassTimer"]?.Value<bool>() ?? false;
            HasPushToPassCooldown = car["hasPushToPassCooldown"]?.Value<bool>() ?? false;
            HasDrsDetection = car["hasDrsDetection"]?.Value<bool>() ?? false;
            HasDrsCount = car["hasDrsCount"]?.Value<bool>() ?? false;
            HasErs = car["hasErs"]?.Value<bool>() ?? false;
            HasBoost = car["hasBoost"]?.Value<bool>() ?? false;
            HasEnginePowerMode = car["hasEnginePowerMode"]?.Value<bool>() ?? false;
            HasDeployMode = car["hasDeployMode"]?.Value<bool>() ?? false;
            HasDeployModeType1 = car["hasDeployModeType1"]?.Value<bool>() ?? false;
            HasDeployModeType2 = car["hasDeployModeType2"]?.Value<bool>() ?? false;
            HasLapDeployLimit = car["hasLapDeployLimit"]?.Value<bool>() ?? false;
            HasThrottleShaping = car["hasThrottleShaping"]?.Value<bool>() ?? false;
            HasFuelMix = car["hasFuelMix"]?.Value<bool>() ?? false;
            HasARBModeP = car["hasARBModeP"]?.Value<bool>() ?? false;
            HasFrontARB = car["hasFrontARB"]?.Value<bool>() ?? false;
            HasExitDiff = car["hasExitDiff"]?.Value<bool>() ?? false;
            HasEntryDiffPreload = car["hasEntryDiffPreload"]?.Value<bool>() ?? false;
            HasTC2 = car["hasTC2"]?.Value<bool>() ?? false;
            HasRearARB = car["hasRearARB"]?.Value<bool>() ?? false;
            HasEntryDiff = car["hasEntryDiff"]?.Value<bool>() ?? false;
            HasTC = car["hasTC"]?.Value<bool>() ?? false;
            HasTwoPartBrakeBias = car["hasTwoPartBrakeBias"]?.Value<bool>() ?? false;
            HasWeightJacker = car["hasWeightJacker"]?.Value<bool>() ?? false;
            HasTwoPartPeakBrakeBias = car["hasTwoPartPeakBrakeBias"]?.Value<bool>() ?? false;
            HasABS = car["hasABS"]?.Value<bool>() ?? false;
            HasFineBrakeBias = car["hasFineBrakeBias"]?.Value<bool>() ?? false;
            HasBrakeBiasMigration = car["hasBrakeBiasMigration"]?.Value<bool>() ?? false;
            HasDryTireCompounds = car["hasDryTireCompounds"]?.Value<bool>() ?? false;
        }

        public override void End(PluginManager pluginManager, benofficial2 plugin)
        {
        }

        public string GetCarBrand(string carId, string carName = "")
        {
            // Check if the car brand is already cached
            if (_carBrands.ContainsKey(carId))
            {
                return _carBrands[carId];
            }

            // Check if the car brand is in the CarInfo JSON
            // Using this method for exceptions that can't have a search token in CarBrandInfo (ex: name is too generic)
            if (_carInfo.Json != null)
            {
                JToken car = _carInfo.Json[carId];
                if (car != null)
                {
                    string brand = car["brand"]?.Value<string>();
                    if (brand != null)
                    {
                        _carBrands[carId] = brand;
                        return brand;
                    }
                }
            }

            // Check if the car brand is in the CarBrandInfo JSON
            // This is inneficient, that's why we cache them in a run-time dictionary
            if (_carBrandInfo.Json != null && _carBrandInfo.Json["car_brands"] is JObject carBrands)
            {
                foreach (var brand in carBrands)
                {
                    if (brand.Value["tokens"] is JArray tokens)
                    {
                        // Check if either carId or carName CONTAINS any of the tokens
                        if (tokens.Any(token => carId.IndexOf(token.ToString(), StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            _carBrands[carId] = brand.Key;
                            return brand.Key;
                        }

                        if (tokens.Any(token => carName.IndexOf(token.ToString(), StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            _carBrands[carId] = brand.Key;
                            return brand.Key;
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string GetCarClassName(string classId)
        {
            // Check if the class name is in the CarClassInfo JSON
            if (_carClassInfo.Json != null && classId != null)
            {
                if (_carClassInfo.Json["car_classes"] is JObject carClasses)
                {
                    JToken carClass = carClasses[classId];
                    if (carClass != null)
                    {
                        return carClass["name"]?.Value<string>() ?? classId;
                    }
                }
            }
            return classId;
        }

        public string GetTireCompoundLetter(int tireCompoundIdx)
        {
            if (TireCompounds == null)
                return string.Empty;

            if (!TireCompounds.TryGetValue(tireCompoundIdx, out string tireCompoundName))
                return string.Empty;

            if (tireCompoundName == null || tireCompoundName.Length == 0)
                return string.Empty;

            // Return the first letter of the compound name as a short representation
            return tireCompoundName[0].ToString();
        }
    }
}
