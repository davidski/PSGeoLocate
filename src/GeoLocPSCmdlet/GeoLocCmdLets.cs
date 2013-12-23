using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Net;
using System.IO;

namespace GeoLocPSCmdlet
{
    // Class inherits from PSCmdlet rather than Cmdlet so that we can
    // access the current working directory path through the SessionState object
    [Cmdlet(VerbsCommon.Get, "GeoLoc")]
    public class GeoLocCmdLets : PSCmdlet
    {
        private string _ipAddress = string.Empty;
        private string _dbPath = string.Empty;             

        // IPAddr parameter is mandatory, and can be read from the pipeline
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Enter a valid IP Address")]
        [Alias("IP")]
        [ValidatePattern(@"\b(?:\d{1,3}\.){3}\d{1,3}\b")]                
        public string IPAddr
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
            try
            {
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
                    ThrowTerminatingError(new ErrorRecord(new Exception("Invalid DBPath"), "Invalid DBPath", ErrorCategory.InvalidArgument, this));
                }

                // MaxMind uses separate lookup services for each of the data files. For this version of the 
                // cmdlet, the available data files are the IP to country, IP to org, and IP to city files
                LookupService lsOrg = new LookupService(_dbPath + "GeoIPOrg.dat", LookupService.GEOIP_STANDARD);
                LookupService lsCity = new LookupService(_dbPath + "GeoIPCity.dat", LookupService.GEOIP_STANDARD);
                //LookupService lsCountry = new LookupService(_dbPath + "GeoIP.dat", LookupService.GEOIP_STANDARD);

                string organization = lsOrg.getOrg(_ipAddress);
                Location l = lsCity.getLocation(_ipAddress);
                //Country c = lsCountry.getCountry(_ipAddress);

                // return a simple GeoLoc object with properties set to the values
                // retrieved by the MaxMind code
                WriteObject(new GeoLoc(organization, l.countryCode, l.countryName, l.region, l.regionName, l.city, l.postalCode, l.latitude, l.longitude, l.metro_code, l.area_code));
            }
            catch (Exception ex)
            {
                // If the MaxMind code throws an exception, we catch it here and wrap it in 
                // a nice PowerShell ErrorRecord. This is most likely to be caused by the MaxMind
                // code being unable to open the data files at the specified location.
                // Note that most of the MaxMind code swallows exceptions and writes error messages to the
                // console, so we are unable to intercept them here.
                ErrorRecord er = new ErrorRecord(ex, "processing-failure", ErrorCategory.InvalidOperation, this);
                ThrowTerminatingError(er);
            }
        }                

        protected override void StopProcessing()
        {
            base.StopProcessing();            
        }
    }
}
