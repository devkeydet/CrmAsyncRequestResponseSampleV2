#TODO: Figure out why this isn't working.  Once you do, replace exe with this
#TODO: make this read from a variable that is a json array of secureconfigs
Install-Module Microsoft.Xrm.Data.Powershell -Force
$conn = "AuthType=Office365;Username=admin@dkdtdevops.onmicrosoft.com;Url=https://dkdtdevops.crm.dynamics.com;Password=Mariak6l9h9898"
$step = Get-CrmRecord -conn $conn -EntityLogicalName sdkmessageprocessingstep -Id "{0d392320-e6b5-e611-80f3-c4346bac2908}" -Fields sdkmessageprocessingstepid,sdkmessageprocessingstepsecureconfigid,plugintypeid
if ($step.sdkmessageprocessingstepsecureconfigid)
{
	#TODO: Update existing secure config
	Write-Output "not null"
}
else
{
	$secureConfigEntityId = New-CrmRecord -conn $conn -EntityLogicalName sdkmessageprocessingstepsecureconfig -Fields @{"secureconfig"="powershell secure config"}
	$processingstepsecureconfig =  Get-CrmRecord -conn $conn -EntityLogicalName sdkmessageprocessingstepsecureconfig -Id $secureConfigEntityId -Fields "*"
	$step.sdkmessageprocessingstepsecureconfigid = $processingstepsecureconfig.EntityReference
	Set-CrmRecord -conn $conn -CrmRecord $step
}