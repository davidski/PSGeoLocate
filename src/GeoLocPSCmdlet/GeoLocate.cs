namespace GeoLocatePSCmdlet
{
    // Simple object with properties to hold the results
    // from a MaxMind query. The query results are put into a 
    // an object to make it easy to work with in PowerShell, 
    // especially in pipelined commands.
    public class GeoLocate
    {
        private string _countryCode = string.Empty;
        private string _countryName = string.Empty;
        private string _subdivisionCode = string.Empty;
        private string _subdivisionName = string.Empty;
        private string _city = string.Empty;
        private string _postalCode = string.Empty;
        private double? _latitude = double.NaN;
        private double? _longitude = double.NaN;

        public GeoLocate(string countryCode, string countryName, string subdivisionCode, string subdivisionName, string city, string postalCode, double? latitude, double? longitude)
        {
            this._countryCode = countryCode;
            this._countryName = countryName;
            this._subdivisionCode = subdivisionCode;
            this._subdivisionName = subdivisionName;
            this._city = city;
            this._postalCode = postalCode;
            this._latitude = latitude;
            this._longitude = longitude;
        }

        public string CountryCode
        {
            get { return _countryCode; }
        }

        public string CountryName
        {
            get { return _countryName; }
        }

        public string SubdivisionCode
        {
            get { return _subdivisionCode; }
        }

        public string SubdivisionName
        {
            get { return _subdivisionName; }
        }

        public string City
        {
            get { return _city; }
        }

        public string PostalCode
        {
            get { return _postalCode; }
        }

        public double? Latitude
        {
            get { return _latitude; }
        }

        public double? Longitude
        {
            get { return _longitude; }
        }


    }
}
