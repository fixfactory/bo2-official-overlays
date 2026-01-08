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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace benofficial2.Plugin
{
    internal class BroadcastMessage
    {
        public enum BroadcastMessageTypes { CamSwitchPos = 0, CamSwitchNum, CamSetState, ReplaySetPlaySpeed, ReplaySetPlayPosition, ReplaySearch, ReplaySetState, ReloadTextures, ChatCommand, PitCommand, TelemCommand };
        public enum PitCommandModeTypes { Clear = 0, WS, Fuel, LF, RF, LR, RR, ClearTires, FastRepair, ClearWS, ClearFR, ClearFuel, TC };

        public class Defines
        {
            public const string BroadcastMessageName = "IRSDK_BROADCASTMSG";
        }

        static IntPtr GetBroadcastMessageID()
        {
            return RegisterWindowMessage(Defines.BroadcastMessageName);
        }

        public static int Broadcast(BroadcastMessageTypes msg, int var1, int var2, int var3)
        {
            return Broadcast(msg, var1, MakeLong((short)var2, (short)var3));
        }

        public static int Broadcast(BroadcastMessageTypes msg, int var1, int var2)
        {
            IntPtr msgId = GetBroadcastMessageID();
            IntPtr hwndBroadcast = IntPtr.Add(IntPtr.Zero, 0xffff);
            IntPtr result = IntPtr.Zero;
            if (msgId != IntPtr.Zero)
            {
                result = PostMessage(hwndBroadcast, msgId.ToInt32(), MakeLong((short)msg, (short)var1), var2);
            }
            return result.ToInt32();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr RegisterWindowMessage(string lpProcName);

        [DllImport("user32.dll")]
        private static extern IntPtr PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public static int MakeLong(short lowPart, short highPart)
        {
            return (int)(((ushort)lowPart) | (uint)(highPart << 16));
        }
    }
}
