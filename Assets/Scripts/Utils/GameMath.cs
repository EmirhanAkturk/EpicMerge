using System;
using UnityEngine;

namespace Utils
{
    public class GameMath
    {
        public static readonly float G = 0.0000000000674f;

        public static string FormatNumber(float number)
        {
            int digitCount = (int)Mathf.Floor(Mathf.Log10(number)) + 1;
            char[] letters = new char[] { 'K', 'M', 'B', 'T', 'q', 'Q', 's', 'S', 'O', 'N', 'd', 'D', 'U', 'V' };


            if (digitCount > 3)
            {
                int letterIndex = (int)Mathf.Floor((digitCount - 1) / 3f) - 1;
                float finalNumber = number / Mathf.Pow(10, 3 * ((int)Mathf.Ceil(digitCount / 3f) - 1));
                finalNumber = (float)Math.Round(finalNumber, 2);
                return finalNumber.ToString() + letters[letterIndex];
            }
            return Math.Round(number, 2).ToString();
        }

        public static Vector3 LerpQuadraticBezier(Vector3 start, Vector3 control, Vector3 end, float t)
        {
            Vector3 a = Vector3.Lerp(start, control, t);
            Vector3 b = Vector3.Lerp(control, end, t);
            return Vector3.Lerp(a, b, t);
        }


        public static bool IsInRange(Vector3 a, Vector3 b, float range, bool inclusive = true)
        {
            float dstSqr = (b - a).sqrMagnitude;
            return inclusive ? dstSqr <= range * range : dstSqr < range * range;
        }
        public static bool IsInRange(Transform a, Transform b, float range, bool inclusive = true)
        {
            float dstSqr = (b.position - a.position).sqrMagnitude;
            return inclusive ? dstSqr <= range * range : dstSqr < range * range;
        }
        public static bool IsOutRange(Vector3 a, Vector3 b, float range, bool inclusive = true)
        {
            float dstSqr = (b - a).sqrMagnitude;
            return inclusive ? dstSqr >= range * range : dstSqr > range * range;
        }
        public static bool IsOutRange(Transform a, Transform b, float range, bool inclusive = true)
        {
            float dstSqr = (b.position - a.position).sqrMagnitude;
            return inclusive ? dstSqr >= range * range : dstSqr > range * range;
        }

        public static Vector3 RandomOnCircleXZ(float radius)
        {
            float randomAngle = UnityEngine.Random.Range(0, 360f) * Mathf.Rad2Deg;
            Vector3 circlePos = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
            return circlePos * radius;
        }

        public static float MapBetweenValues(float val, float valMin, float valMax, float targetMin, float targetMax)
        {
            return (val - valMin) / (valMax - valMin) * (targetMax - targetMin) + targetMin;
        }
    
        public static int GetDigit(double number)
        {
            if (number < 1.01f)
                return 1;
        
            int count = 0;
            while (number > 1) 
            {
                number /= 10;
                ++count;
            }
        
            return count;
        }
    }
}
