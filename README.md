# IpGeoFencing
A simple and easily extensible IP geofencing engine

## Overview
Geofencing is the use of virtual geographic perimeters to determine the behavior of an application.
Provided a specific coordinate (latitude and longitude), a geofencing engine can determine to which virtal geographic area or areas the location belongs, and consequentially take action.

The IP geofencing engine in this project run based on specific IP addresses.
The engine would first translate the IP address into a geographic location, using the free CSV database provided by IP2Location
[CSV database](https://lite.ip2location.com/database/db11-ip-country-region-city-latitude-longitude-zipcode-timezone "[https://geojson.org/](https://lite.ip2location.com/database/db11-ip-country-region-city-latitude-longitude-zipcode-timezone)") (included in the project).

The engine then determines which of the configured geographic areas contain the location, and for each match fires the correspondent configured action.
Geographic areas can be configured in the engine by importing a file in [GeoJSON format](https://geojson.org/ "https://geojson.org/").
Sample GeoJSON files are included in this project for testing purposes, and also freely available for the public on the [OpenDataSoft website](https://public.opendatasoft.com/explore/ "https://public.opendatasoft.com/explore/").

## Use case scenarios
This engine is particularly useful in cases when the ip address is available, but not the coordinate.<br />
These are the typical scenarios in which web applications can use the engine with the ip address of the http requests:

- Blocking requests from black-listed geographic areas
- Enabling/disabling features for specific regions
- Localizing content
- Finding the availability of products or service nearby the ip location

## Anatomy of a geofenging rule
A rule is constructed from 3 elements: name, predicate and action.

- rule name: short description for the rule

- predicate: a boolean function that determines if the rule should be applied, The function will receive as input the list of geographic areas that contain the IP address location (normally only one area if the areas do not overlap), plus the IP address and its location info.
```csharp
predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "New York"); }
```
- action: a routine that will be executed only if the predicate is evaluated as true. Same inputs as the predicate
```csharp
action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in New York State!"); })
```

In addition to regular rules, ***default rules*** can also be specified.
Default rules are executed if and only if the IP address location is *not* contained in any of the geographic areas available.
Unlike regular rules, default rules only need the action to be constructed


## Configuring the geofencing engine
```csharp
var engine = new IPGeoFencingEngineBuilder()
	.AddIP2LocationFromCSVFile(@"\\geofencing\data\IP2LOCATION-LITE-DB11.CSV")
	.AddGeographicAreasFromGeoJSONFile(@"\\geofencing\data\demo.geojson")
	.AddRule("New York", 
		predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "New York"); },	
		action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in New York State!"); })
	.AddRule("Montana",
		predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Montana"); },
		action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in Montana!"); })
	.AddRule("Billings",
		predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Billings"); },
		action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in Billings, MT"); })
	.AddRule("Montana but not Billings",
		predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Montana") && !areas.Any(A => A.Name == "Billings"); },
		action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in Montana but not in Billings!"); })
	.AddRule("New York or Montana",
		predicate: (areas, ip, location) => { return areas.Any(A => A.Name == "Montana") || areas.Any(A => A.Name == "New York"); },
		action: (areas, ip, location) => { Console.WriteLine($"The IP Address: {ip} is in New York State or Montana!"); })
	.AddDefaultAction((ip, loc) => Console.WriteLine($"The IP Address: {ip} is outside all the areas provided"))
	.Build();
```
## Running the engine
```csharp

engine.Run("98.127.147.57"); //Billings, MT IP Address
//The IP Address: 98.127.147.57 is in Montana!
//The IP Address: 98.127.147.57 is in Billings, MT
//The IP Address: 98.127.147.57 is in New York State or Montana!

engine.Run("172.254.112.210"); //New York, NY IP Address
//The IP Address: 172.254.112.210 is in New York State!
//The IP Address: 172.254.112.210 is in New York State or Montana!

engine.Run("157.240.3.35"); //Seattle, WA IP Address
//The IP Address: 157.240.3.35 is outside all the areas provided
```

## Running the demo
The solution comes with a simple demo console app that demonstrates the engine configuration and execution.
Before you run the demo, make sure to edit the *appsettings.json* file included in th eproject and set the correct full path to the data folder in your file system.
Example:
```json
{
  "DataFolder": "D:\\IPGeoFencing\\Data"
}
```

## Extending the engine
The core services used by the engine that can be easily extended to enrich its functionality:

### IIP2LocationProvider
This interface provides the engine with the translation service to obtain the grographic location from the ip address.
The project provides out-of-the-box a simple implementation where the Ip2Location CSV data is loaded entirely in memory and the ip translation occurrs via Linq queries. More efficient implementations can be added that utilize databases queries/indexes to look up the locations, or leverage the IP2Location API for a lightweight solution, etc.
```csharp
public interface IIP2LocationProvider
{
	LocationModel? GetLocationFromIP(IPAddress ipAddress);
	LocationModel? GetLocationFromIP(long ipAddress);
}
```

### IGeographicAreasProvider
This interface is used by the engine to determine which geographic areas contain a given coordinate point.
The default implementation in the project is loading in memory the full collection of geographic areas (in the shape of polygons or circles) that need to be evaluated. Like the IIP2LocationProvider, more efficient implementations can be added to leverage databases, particularly those supporting geospatial queries (e.g. MongoDB).
Moreover, the project currently only supports the GeoJSON file format to import geographic areas, however there are other popular formats that can be integrated: Shapefile (GIS), KML (Google Earth), etc. 
```csharp
public interface IGeographicAreasProvider
{
	IEnumerable<GeographicAreaModel> GetAreasContaining(GeoCoordinate point);
}
```

### Plugging in your own service implementations
The engine builder already allows developers to plug in their own service implementations:
```csharp
        IIP2LocationProvider myIP2LocationProvider = new myOwnIP2LocationProviderImplementation();
        IGeographicAreasProvider myGeographicAreasProvider = new myOwnGeographicAreasProviderImplementation();

        var engine = new IPGeoFencingEngineBuilder()
            .AddIP2LocationProvider(myIP2LocationProvider)
            .AddGeographicAreasProvider(myGeographicAreasProvider)
	    .AddRule( ...
```

