using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoLocPSCmdlet
{
    // Simple object with three properties to hold the results
    // from a MaxMind query. The query results are put into a 
    // an object to make it easy to work with in PowerShell, 
    // especially in pipelined commands.
    public class GeoLoc
    {
        private string _Organization = string.Empty;
        private string _countryCode = string.Empty;
        private string _countryName = string.Empty;
        private string _region = string.Empty;
        private string _regionName = string.Empty;
        private string _city = string.Empty;
        private string _postalCode = string.Empty;
        private double _latitude = double.NaN;
        private double _longitude = double.NaN;
        private float _metro_code = float.NaN;
        private float _area_code = float.NaN;

        public GeoLoc(string Organization, string countryCode, string countryName, string region, string regionName, string City, string postalcode, double latitude, double longitude, float metro_code, float area_code)
        {
            this._Organization = Organization;
            this._countryCode = countryCode;
            this._countryName = countryName;
            this._region = region;
            this._regionName = regionName;
            this._city = City;
            this._postalCode = postalcode;
            this._latitude = latitude;
            this._longitude = longitude;
            this._metro_code = metro_code;
            this._area_code = area_code;

        }

        public string Organization
        {
            get { return _Organization; }
        }

        public string CountryCode
        {
            get { return _countryCode; }
        }

        public string CountryName
        {
            get { return _countryName; }
        }

        public string Region
        {
            get { return _region; }
        }

        public string RegionName
        {
            get { return _regionName; }
        }

        public string City
        {
            get { return _city; }
        }

        public string PostalCode
        {
            get { return _postalCode; }
        }

        public double Latitude
        {
            get { return _latitude; }
        }

        public double Longitude
        {
            get { return _longitude; }
        }

        public float MetroCode
        {
            get { return _metro_code; }
        }

        public float AreaCode
        {
            get { return _area_code; }
        }

    }
}
