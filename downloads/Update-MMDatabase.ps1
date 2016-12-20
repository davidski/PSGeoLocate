function Update-MMDatabase {
    <#
    .SYNOPSIS
        Update the MaxMind database from the web, if required
    .DESCRIPTION
        Takes a hash of the existing GeoIP.dat file and submits it to the MaxMind update check webservice.
        If a new file is available, retrieve the main country and the organizational data files and replace the
        existing database files with current copies.
    .PARAMETER DBPath
        Fully qualified path to the database directory. Defaults to D:\Program Files\SeattleChildrens\GeoLoc
    .NOTES
        This needs to be altered to do the update call to a temp file, then check the temp file to see if the size
        warrents an update. This would avoid having to call the update service twice, potentially pulling the main
        DB file multiple times.
#>
    [cmdletbinding()]
    Param(
       [string]
       $DBPath="D:\GeoIp"
    )

    $configuration = (Get-Content D:\scripts\parameters.conf) -join "`n" | ConvertFrom-Json
    $license_key = $configuration.MaxMind_License_Key

    #calculate the MD5 hash (w/o dashes) of the existing file
    $md5 = new-object -TypeName System.Security.Cryptography.MD5CryptoServiceProvider
    $hash = [System.BitConverter]::ToString($md5.ComputeHash([System.IO.File]::ReadAllBytes("$DBPath\GeoIP.dat")))
    $hash = $hash -replace "-", ""
    Write-Verbose "Hash is $hash"

    #check if the file has been updated
    Write-Verbose "Checking update service."
    Invoke-RestMethod -uri "http://updates.maxmind.com/app/update?license_key=$license_key&md5=$hash" -OutFile "$DBPath\GeoIP.gz"
    
    if ((Get-ChildItem "$DBPath\GeoIP.gz").length -gt 10000) {
        Write-Verbose "Found an update."
        
        #Extract the main GeoIP file
        Write-Verbose "Expanding main GeoIP data file."
        Expand-Archive "$DBPath\GeoIP.gz" -EntryPath "GeoIP.dat" -FlattenPath -Force -OutputPath $DBPath
        Remove-Item "$DBPath\GeoIP.gz"

        #assume that the organizational DB has been updated and pull a copy of that down as well
        Invoke-RestMethod -uri "http://download.maxmind.com/app/download_new?edition_id=111&suffix=tar.gz&license_key=$license_key" -OutFile "$DBPath\GeoIPorg.tar.gz"
    
        #Extract the organizational file
        Write-Verbose "Expanding GeoIP organizational data file."
        Expand-Archive "$DBPath\GeoIPorg.tar.gz" -OutputPath $DBPath
        Read-Archive "$DBPath\GeoIPorg.tar" | where name -like "*.dat" | Expand-Archive -Flattenpaths -OutputPath $DBPath -force
        Remove-Item "$DBPath\GeoIPorg.tar.gz"
        Remove-Item "$DBPath\GeoIPorg.tar"

        #Pull down the city DB
        Invoke-RestMethod -uri "http://download.maxmind.com/app/download_new?edition_id=133&suffix=tar.gz&license_key=$license_key" -OutFile "$DBPath\GeoIPcity.tar.gz"
    
        #Extract the city file
        Write-Verbose "Expanding GeoIP city data file."
        Expand-Archive -Path "$DBPath\GeoIPcity.tar.gz" -FlattenPaths -ShowProgress -OutputPath $DBPath
        Read-Archive "$DBPath\GeoIPcity.tar" | where name -like "*.dat" | Expand-Archive -Flattenpaths -OutputPath $DBPath -force
        Remove-Item "$DBPath\GeoIPcity.tar.gz"
        Remove-Item "$DBPath\GeoIPcity.tar"
   
    } else {
        Write-Verbose "No update required."
    }

}