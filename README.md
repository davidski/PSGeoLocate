[![Build Status](https://ci.appveyor.com/api/projects/status/github/davidski/PSGeoLocate?svg=true)](https://ci.appveyor.com/project/davidski/PSGeoLocate)

# Introduction

PowerShell module for working with MaxMind (http://www.maxmind.com) GeoIP2 databases.

# Installation

Currently suboptimal.

1. Download the DLL to your working directory
2. `Import-Module .\PSGeoLocate.dll'
3. Profit!

# Use

Will operate against either the GeoIP2 or the GeoLite2 datasets and return 
a universal GeoLocation object with the IP address and a subset of the 
data fields from the GeoIP database. Fields not present in the provided database 
file (such as city name when querying the Country database) will be returned as blank.

The database files can be in any accessible directory, but must be in the 
original naming scheme as downloaded from MaxMind. This cmdlet uses the filename to determine 
the database type it is querying. Supplying an unknown database will cause a 
(graceful) error.

A persistent connection to the database is used for an entire pipeline. To maximize 
perfomance, use the PowerShell pipelline to pass as many IP Addresses as possible in a 
single batch. See `ProcessGeoLocs` for an example.

# Command Set

Exposed cmdlets include (check `Get-Command -Module PSGeoLocate` for latest):

+ Get-GeoLocation - Lookup a given IP Address against a MaxMind database
+ ProcessGeoLocs - Process a CSV and add MaxMind Organization, Country Code, and Country Name fields
+ Update-MMDatabase - Update MaxMind database files
