[![Build Status](https://ci.appveyor.com/api/projects/status/github/davidski/PSGeoLocate?svg=true)](https://ci.appveyor.com/project/davidski/PSGeoLocate)

# Introduction

PowerShell module for working with MaxMind (http://www.maxmind.com) geolocation binary (legacy) database.

# Requirements

+ PowerShell Community Extensions (http://pscx.codeplex.com/)

# Command Set

Exposed cmdlets include (check `Get-Command -Module PSGeoLocate` for latest):

+ Get-GeoLoc - Lookup a given IP Address against the MaxMind database
+ ProcessGeoLocs - Process a CSV and add MaxMind Organization, Country Code, and Country Name fields
+ Update-MMDatabase - Update MaxMind database files
