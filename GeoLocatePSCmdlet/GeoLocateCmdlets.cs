using System;
using System.Management.Automation;
using System.IO;
using System.Net;
using MaxMind.GeoIP2;


[assembly: CLSCompliant(true)]
namespace GeoLocate.Command
{

    [Cmdlet(VerbsCommon.Get, "GeoLocation")]
    [CLSCompliant(false)]
    [OutputType("GeoLocation")]
    public class GeoLocateCommand : Cmdlet
    {
        private IPAddress _ipAddress;
        private string _dbPath = string.Empty;
        private dbType _dbType = dbType.City;
        private DatabaseReader reader;

        // IPAddress parameter is mandatory and can be read from the pipeline
        [System.Management.Automation.Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Enter a valid IP Address")]
        [ValidatePattern(@"\b(?:\d{1,3}\.){3}\d{1,3}\b")]
        public IPAddress IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        // Path parameter is mandatory and cannot be read from the pipeline
        [System.Management.Automation.Parameter(Position = 1, Mandatory = true, ValueFromPipeline = false, HelpMessage = "Enter a valid path to a MaxMind data file")]
        [Alias("PSPath")]
        public string Path
        {
            get { return _dbPath; }
            set { _dbPath = value; }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            
            // figure out the DBType based upon the passed file name
            var dbFilename = System.IO.Path.GetFileNameWithoutExtension(_dbPath);
            string[] parts = dbFilename.Split('-');
            try
            {

                _dbType = (dbType)Enum.Parse(typeof(dbType), parts[1]);
            }
            catch (ArgumentException)
            {
                ThrowTerminatingError(new ErrorRecord(new ArgumentException("Unable to determine database type for " + _dbPath), "Invalid database type", ErrorCategory.InvalidArgument, this));
            }

            // verify that the specified path is valid
            if (!File.Exists(_dbPath))
            {
                // if path invalid, toss a PowerShell ErrorRecord and terminate
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("Invalid Database Path"), "Invalid database path", ErrorCategory.InvalidArgument, this));
            }

            // While a common lookup service is used for all database types, the response types differ
            this.reader = new DatabaseReader(_dbPath);

        }

        protected override void ProcessRecord()
        {

            // call the proper reader method based upon the dbType
            // return a GeoLocation object with properties set to the matching returned values
            switch (_dbType)
            {
                // Country database
                case dbType.Country:
                    var cresponse = reader.Country(_ipAddress);
                    GeoLocation clocation = new GeoLocation(
                        ipAddress: _ipAddress,
                        countryCode: cresponse.Country.IsoCode,
                        countryName: cresponse.Country.Name
                    );
                    WriteObject(clocation);
                    break;

                // ISP database
                case dbType.ISP:
                    var ispresponse = reader.Isp(_ipAddress);
                    GeoLocation isplocation = new GeoLocation(
                        ipAddress: _ipAddress
                    );
                    WriteObject(isplocation);
                    break;


                // ISP database
                case dbType.Domain:
                    var domainresponse = reader.Domain(_ipAddress);
                    GeoLocation domainlocation = new GeoLocation(
                        ipAddress: _ipAddress,
                        domain: domainresponse.Domain
                    );
                    WriteObject(domainlocation);
                    break;

                // Enterprise database
                case dbType.Enterprise:
                    var entresponse = reader.Enterprise(_ipAddress);
                    GeoLocation entlocation = new GeoLocation(
                        ipAddress: _ipAddress,
                        countryCode: entresponse.Country.IsoCode,
                        countryName: entresponse.Country.Name,
                        subdivisionCode: entresponse.MostSpecificSubdivision.IsoCode,
                        subdivisionName: entresponse.MostSpecificSubdivision.Name,
                        city: entresponse.City.Name,
                        latitude: entresponse.Location.Latitude,
                        longitude: entresponse.Location.Longitude
                    );
                    WriteObject(entlocation);
                    break;

                // All others (defaults to City)
                default:
                    var cityresponse = reader.City(_ipAddress);
                    GeoLocation citylocation = new GeoLocation(
                        ipAddress: _ipAddress,
                        countryCode: cityresponse.Country.IsoCode,
                        countryName: cityresponse.Country.Name,
                        subdivisionCode: cityresponse.MostSpecificSubdivision.IsoCode,
                        subdivisionName: cityresponse.MostSpecificSubdivision.Name,
                        city: cityresponse.City.Name,
                        latitude: cityresponse.Location.Latitude,
                        longitude: cityresponse.Location.Longitude
                    );
                    WriteObject(citylocation);
                    break;
            }

        }

        // Called upon the normal end of processing
        protected override void EndProcessing()
        {
            base.EndProcessing();
            reader.Dispose();
        }

        // Called upon pipeline termination (user CTRL+C or error)
        protected override void StopProcessing()
        {
            base.StopProcessing();
            reader.Dispose();
        }

    }

}