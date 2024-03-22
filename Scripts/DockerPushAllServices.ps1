$totalSteps = 5

Function LogStep{
    param (
        [int] $step,
        [string] $serviceName
    )
    Write-Host "";
    Write-Host "=======================Pushing ${serviceName} ${step}/${totalSteps}=======================";    
    Write-Host "";
}

LogStep -step 1 -serviceName "AccountingService"
docker push krz123/homeaccountant.accountingservice
LogStep -step 2 -serviceName "Gateway"
docker push krz123/homeaccountant.gateway
LogStep -step 3 -serviceName "IdentityPlatform"
docker push krz123/homeaccountant.identityplatform
LogStep -step 4 -serviceName "FriendsService"
docker push krz123/homeaccountant.friendsservice
LogStep -step 5 -serviceName "CategoriesService"
docker push krz123/homeaccountant.categoriesservice