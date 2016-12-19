using System;
using System.Management.Automation;
using System.IO;
using MaxMind.GeoIP2;

[assembly: CLSCompliant(true)]
namespace GeoLocatePSCmdlet
{
    // Class inherits from PSCmdlet rather than Cmdlet so that we can
    // access the current working directory path through the SessionState object
    [Cmdlet(VerbsCommon.Get, "GeoLoc")]
    [CLSCompliant(false)]
    public class GeoLocateCmdlet : PSCmdlet
    {
        private string _ipAddress = string.Empty;
        private string _dbPath = string.Empty;

        // IPAddress parameter is mandatory and can be read from the pipeline
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Enter a valid IP Address")]
        [ValidatePattern(@"\b(?:\d{1,3}\.){3}\d{1,3}\b")]
        public string IPAddress
        {
            get { return _ipAddress; }
            set { _ipAddress = value; }
        }

        // DBPath parameter is optional, and is only read from the command line, not the pipeline. Reading from the pipeline would
        // require use of pipelined objects with named IPAddr and DBPath properties.
        [Parameter(Position = 1, ValueFromPipeline = false, HelpMessage = "Enter a valid absolute or network shared DB path")]
        public string DBPath
        {
            get { return _dbPath; }
            set { _dbPath = value; }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
            // if no DBPath parameter was provided, use the current working directory
            if (String.IsNullOrEmpty(_dbPath))
            {

                _dbPath = SessionState.Path.CurrentFileSystemLocation.Path;
            }

            if (!_dbPath.EndsWith(@"\"))
            {
                _dbPath = _dbPath + @"\";
            }

            // verify that the specified path is valid
            if (!Directory.Exists(_dbPath))
            {
                // if not, toss a PowerShell ErrorRecord and terminate
                ThrowTerminatingError(new ErrorRecord(new DirectoryNotFoundException("Invalid Database Path"), "Invalid database path", ErrorCategory.InvalidArgument, this));
            }

            // MaxMind uses separate lookup services_ for each of the data files. For this version of the 
            // cmdlet, the available data files are the Country and the City database
            var reader = new DatabaseReader(_dbPath + "GeoIP2-City.mmdb");
            //var reader = new DatabaseReader(_dbPath + "GeoIP2-Country.mmdb"))

            var response = reader.City(_ipAddress);

            // return a simple GeoLoc object with properties set to the values
            // retrieved by the MaxMind library
            WriteObject(new GeoLocate(response.Country.IsoCode, response.Country.Name, response.MostSpecificSubdivision.IsoCode, response.MostSpecificSubdivision.Name, response.City.Name, response.Postal.Code, response.Location.Latitude, response.Location.Longitude));
        }

        protected override void StopProcessing()
        {
            base.StopProcessing();
        }
    }
}