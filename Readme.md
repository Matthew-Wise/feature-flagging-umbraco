# Our.FeatureFlags

Enables you to use [Microsoft.FeatureManagement](https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core?tabs=core5x) to not only control your front end by these filters and flags but also your back office properties.

These properties can still be mandatory and have validation.

# Installing

Follow the install step(s) for [Microsoft.FeatureManagement](https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core?tabs=core5x).

The package will register its dependencies using a Composer so no additional configuration is needed.


# Using

Create the data type you want to wrap then create a Feature Flagged data type that wraps it. Within your code use the FeatureManager as per the [Microsoft.FeatureManagement](https://docs.microsoft.com/en-us/azure/azure-app-configuration/use-feature-flags-dotnet-core?tabs=core5x) documentation.

Properties work with Models builder as if there were not feature flagged.

# Known Issues and work arounds

## HasValue check
Uses the base property value converter method of checking this, as I am unable to get the correct converter on this request. 

I would suggest never using HasValue and instead null check etc the strongly type value.