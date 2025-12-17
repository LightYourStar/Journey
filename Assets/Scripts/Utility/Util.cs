using System;
using UnityEngine;

namespace Game.Utility
{
    internal static class Util
    {
        public static void RunIfKeyDown(KeyCode _key, Action _action)
        {
            if (Input.GetKeyDown(_key))
                _action.Invoke();
        }

        public static int Range(int _value, int _inclMin, int _exclMax)
        {
            if (IsRange(_value, _inclMin, _exclMax))
                return _value;

            throw new ArgumentOutOfRangeException($"value '{_value}' is out of the bounds, '{_inclMin}' to '{_exclMax}'");
        }

        public static int UpperLimit(int _value, int _exclMax)
        {
            if (IsMax(_value, _exclMax))
                return _value;

            throw new ArgumentOutOfRangeException($"value '{_value}' is out of bounds, (max = '{_exclMax}')");
        }

        public static int LowerLimit(int _value, int _inclMin)
        {
            if (IsMin(_value, _inclMin))
                return _value;

            throw new ArgumentOutOfRangeException($"value '{_value}' is out of bounds, (min = '{_inclMin}')");
        }


        public static bool IsMin(int _value, int _inclMin) => _value >= _inclMin;
        public static bool IsMax(int _value, int _exclMax) => _value < _exclMax;
        public static bool IsRange(int _value, int _inclMin, int _exclMax) => IsMin(_value, _inclMin) && IsMax(_value, _exclMax);
    }
}
