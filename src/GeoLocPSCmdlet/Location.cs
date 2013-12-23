using System;
using System.IO;

namespace GeoLocPSCmdlet
{
    // MaxMind-supplied code
    // not used in current version of cmdlet, but ported in for future use if 
    // Children's expands their MaxMind subscription

    public class Location
    {
        public String countryCode;
        public String countryName;
        public String region;
        public String city;
        public String postalCode;
        public double latitude;
        public double longitude;
        public int dma_code;
        public int area_code;
        public String regionName;
        public int metro_code;

        private static double EARTH_DIAMETER = 2 * 6378.2;
        private static double PI = 3.14159265;
        private static double RAD_CONVERT = PI / 180;

        public double distance(Location loc)
        {
            double delta_lat, delta_lon;
            double temp;

            double lat1 = latitude;
            double lon1 = longitude;
            double lat2 = loc.latitude;
            double lon2 = loc.longitude;

            // convert degrees to radians
            lat1 *= RAD_CONVERT;
            lat2 *= RAD_CONVERT;

            // find the deltas
            delta_lat = lat2 - lat1;
            delta_lon = (lon2 - lon1) * RAD_CONVERT;

            // Find the great circle distance
            temp = Math.Pow(Math.Sin(delta_lat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(delta_lon / 2), 2);
            return EARTH_DIAMETER * Math.Atan2(Math.Sqrt(temp), Math.Sqrt(1 - temp));
        }
    }


}
