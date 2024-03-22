$totalSteps = 5

Function LogStep{
    param (
        [int] $step,
        [string] $serviceName
    )
    Write-Host "";
    Write-Host "=======================Building ${serviceName} ${step}/${totalSteps}=======================";    
    Write-Host "";
}

LogStep -step 1 -serviceName "AccountingService"
cd ..
docker build -t krz123/homeaccountant.accountingservice --no-cache -f .\Services\HomeAccountant.AccountingService\Dockerfile .
LogStep -step 2 -serviceName "Gateway"
docker build -t krz123/homeaccountant.gateway --no-cache -f .\APIGateway\HomeAccountant.Gateway\Dockerfile .
LogStep -step 3 -serviceName "IdentityPlatform"
docker build -t krz123/homeaccountant.identityplatform --no-cache -f .\Services\HomeAccountant.IdentityPlatform\Dockerfile .
LogStep -step 4 -serviceName "FriendsService"
docker build -t krz123/homeaccountant.friendsservice --no-cache -f .\Services\HomeAccountant.FriendsService\Dockerfile .
LogStep -step 5 -serviceName "CategoriesService"
docker build -t krz123/homeaccountant.categoriesservice --no-cache -f .\Services\HomeAccountant.CategoriesService\Dockerfile .