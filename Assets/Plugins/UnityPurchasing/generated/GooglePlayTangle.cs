#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("41HS8ePe1dr5VZtVJN7S0tLW09A0td0qNNczai1WhLUekqZCZQsofbo0HDdvSE5R9KI8JkG15Hq1WblBn0dbMUYSM8QswW9LU6ICd63xPRzKrcy4JZP92LNq6kNnjlPxztUwTWxSd8jCr2SxkEY8eF8G8YMI8HhW63BRWDq2sPDTkvnRLjhY5vW6fX/VJMmD5dJoj0NyJyZjJUjleVoBX4ZiqLkAib154rciJGbfFKfzY/q4ZwGK5ZC7xOzPMENW6seV9kHVETkDMpqQynWKXrVgwRtFZSpquJ4dlFHS3NPjUdLZ0VHS0tMXYt/xxzJyftKQgqWsZ7593ywSjwt+qKASFsHZwd9nWTMMYR9Y8NG8loMJXbu9UBO6/3xXSsRLWNHQ0tPS");
        private static int[] order = new int[] { 0,8,13,12,12,7,9,11,13,9,10,13,13,13,14 };
        private static int key = 211;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
