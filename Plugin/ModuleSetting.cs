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
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace benofficial2.Plugin
{
    public abstract class ModuleSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void NotifyAllPropertiesChanged()
        {
            // Use reflection to get all public properties of the class
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                OnPropertyChanged(property.Name);
            }
        }
    }

    public class ModuleSettingFloat : INotifyPropertyChanged
    {
        private float _value = 0.0f;
        private string _valueString = "0";
        private bool _valid = true;

        public ModuleSettingFloat(float initialValue = default)
        {
            Value = initialValue;
        }        

        public float Value
        {
            get { return _value; }
            set 
            {
                if (_value != value)
                {
                    _value = value;
                    _valueString = value.ToString(CultureInfo.InvariantCulture);
                    Valid = true;
                }
            }
        }
        public string ValueString
        {
            get { return _valueString; }
            set
            {
                if (_valueString != value)
                {
                    _valueString = value;

                    // Convert to float when set
                    if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
                    {
                        _value = result;
                        Valid = true;
                    }
                    else
                    {
                        _value = 0;
                        Valid = false;
                    }
                }
            }
        }
        public bool Valid
        {
            get { return _valid; }
            private set
            {
                if (_valid != value)
                {
                    _valid = value;
                    OnPropertyChanged(nameof(Valid));
                }
            }
        }

        // Implicit conversion to make assignment to float work seamlessly
        public static implicit operator float(ModuleSettingFloat setting) => setting.Value;

        // Implicit conversion to allow direct assignment from float
        public static implicit operator ModuleSettingFloat(float value) => new ModuleSettingFloat(value);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
