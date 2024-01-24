$totalSteps = 1

Function LogStep{
    param (
        [int] $step,
        [string] $serviceName
    )
    Write-Host "";
    Write-Host "=======================Building ${serviceName} ${step}/${totalSteps}=======================";    
    Write-Host "";
}

$Environment = Read-Host -Prompt 'Podaj srodowisko(Development/Production)'

LogStep -step 1 -serviceName "Frontend"
docker build -t krz123/homeaccountant.frontend --no-cache --build-arg Environment=$Environment -f .\Frontend\HomeAccountant\Dockerfile .