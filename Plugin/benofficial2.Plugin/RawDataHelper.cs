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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    public static class RawDataHelper
    {
        public static bool TryGetSessionData<T>(ref GameData data, out T result, params object[] path)
        {
            result = default;
            dynamic raw = data.NewData.GetRawDataObject();
            if (raw == null)
                return false;

            try
            {
                if (raw.AllSessionData is IDictionary<object, object> allSessionData)
                {
                    return TryGetValue<T>(allSessionData, out result, path);
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
                if (raw.AllSessionData is IDictionary<object, object> allSessionData)
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
                if (raw.Telemetry is IDictionary<string, object> telemetry)
                {
                    return TryGetValue<T>(telemetry, out result, path);
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
