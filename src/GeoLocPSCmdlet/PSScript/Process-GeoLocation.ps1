Param([parameter(Mandatory=$true)]
	[ValidateNotNullOrEmpty()]	
	[string]$CSVPath, 
	[parameter(Mandatory=$false)]
	[string]$DBPath) 


$list = Import-CSV $CSVPath

#Add Columns
$list | Add-Member -name "org" -value "" -MemberType NoteProperty -force
$list | Add-Member -name "country_code" -value "" -MemberType NoteProperty -force
$list | Add-Member -name "country_name" -value "" -MemberType NoteProperty -force

Import-Module GeoLocate
 
$maxmind = New-MMConnection -DBPath $dbpath -Type Enterprise

$NewCSVObject = @() 
foreach ($item in $list)
{  
    $geoloc = Get-GeoLocation -Connection $maxmind -IPAddress $item.s_ip
	$item.org = $geoloc.Org
	$item.country_code = $geoloc.CountryCode
	$item.country_name = $geoloc.CountryName
    $NewCSVObject += $item
} 
$NewCSVObject | export-csv $CSVPath -noType -force
