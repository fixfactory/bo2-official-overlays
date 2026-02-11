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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    public static class RawDataHelper
    {
        // Cache the last raw root object and its AllSessionData dictionary to avoid
        // repeated dynamic lookups when called multiple times within the same frame.
        private static object s_cachedAllSessionDataRoot = null;
        private static IDictionary<object, object> s_cachedAllSessionData = null;
        private static object s_cachedTelemetryRoot = null;
        private static IDictionary<string, object> s_cachedTelemetry = null;

        public static bool TryGetSessionData<T>(ref GameData data, out T result, params object[] path)
        {
            result = default;
            dynamic raw = data.NewData.GetRawDataObject();
            if (raw == null)
                return false;

            try
            {
                // If the raw root object is the same as last call, reuse the cached
                // AllSessionData dictionary to avoid repeated dynamic property access.
                if (ReferenceEquals(raw, s_cachedAllSessionDataRoot) && s_cachedAllSessionData != null)
                {
                    return TryGetValue<T>(s_cachedAllSessionData, out result, path);
                }

                if (raw.AllSessionData is IDictionary<object, object> allSessionData)
                {
                    s_cachedAllSessionDataRoot = raw;
                    s_cachedAllSessionData = allSessionData;
                    return TryGetValue<T>(allSessionData, out result, path);
                }
                else
                {
                    // Clear cache for this raw root when AllSessionData isn't present.
                    s_cachedAllSessionDataRoot = raw;
                    s_cachedAllSessionData = null;
                }
            }
            catch { Debug.Assert(false); }
            return false;
        }

        public static bool TryGetFirstSessionData<T>(ref GameData data, out T result, IEnumerable<object[]> paths)
        {
            result = default;
            dynamic raw = data.NewData.GetRawDataObject();
            if (raw == null)
                return false;

            try
            {
                IDictionary<object, object> allSessionData = null;

                if (ReferenceEquals(raw, s_cachedAllSessionDataRoot) && s_cachedAllSessionData != null)
                {
                    allSessionData = s_cachedAllSessionData;
                }
                else if (raw.AllSessionData is IDictionary<object, object> asd)
                {
                    s_cachedAllSessionDataRoot = raw;
                    s_cachedAllSessionData = asd;
                    allSessionData = asd;
                }

                if (allSessionData != null)
                {
                    result = default;
                    foreach (var path in paths)
                    {
                        if (TryGetValue<T>(allSessionData, out result, path))
                            return true;
                    }
                }
            }
            catch { Debug.Assert(false); }
            return false;
        }

        public static bool TryGetTelemetryData<T>(ref GameData data, out T result, params object[] path)
        {
            result = default;
            dynamic raw = data.NewData.GetRawDataObject();
            if (raw == null)
                return false;

            try
            {
                // Reuse cached telemetry dictionary when the raw telemetry root is the same
                if (ReferenceEquals(raw, s_cachedTelemetryRoot) && s_cachedTelemetry != null)
                {
                    return TryGetValue<T>(s_cachedTelemetry, out result, path);
                }

                if (raw.Telemetry is IDictionary<string, object> telemetry)
                {
                    s_cachedTelemetryRoot = raw;
                    s_cachedTelemetry = telemetry;
                    return TryGetValue<T>(telemetry, out result, path);
                }
                else
                {
                    // Clear telemetry cache for this raw root when Telemetry isn't present.
                    s_cachedTelemetryRoot = raw;
                    s_cachedTelemetry = null;
                }
            }
            catch { Debug.Assert(false); }
            return false;
        }

        public static bool TryGetValue<T>(object root, out T result, params object[] path)
        {
            result = default;

            if (root == null || path == null || path.Length == 0)
                return false;

            object current = root;

            foreach (var key in path)
            {
                switch (current)
                {
                    case Dictionary<object, object> dictObj when dictObj.TryGetValue(key, out var valueObj):
                        current = valueObj;
                        break;

                    case Dictionary<string, object> dictStr when key is string strKey && dictStr.TryGetValue(strKey, out var valueStr):
                        current = valueStr;
                        break;

                    case List<object> list when key is int index && index >= 0 && index < list.Count:
                        current = list[index];
                        break;

                    case Array array when key is int arrIndex && arrIndex >= 0 && arrIndex < array.Length:
                        current = array.GetValue(arrIndex);
                        break;

                    default:
                        return false;
                }
            }

            if (current is T casted)
            {
                result = casted;
                return true;
            }

            try
            {
                result = (T)Convert.ChangeType(current, typeof(T));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
