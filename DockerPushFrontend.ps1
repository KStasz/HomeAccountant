$totalSteps = 1

Function LogStep{
    param (
        [int] $step,
        [string] $serviceName
    )
    Write-Host "";
    Write-Host "=======================Pushing ${serviceName} ${step}/${totalSteps}=======================";    
    Write-Host "";
}

LogStep -step 1 -serviceName "Frontend"
docker push krz123/homeaccountant.frontend