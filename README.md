# IpGeoFencing
A simple and easily extensible IP geo-fencing engine

## Configuring the geo-fencing engine
```csharp
var engine = new IPGeoFencingEngineBuilder()
	.AddIP2LocationFromCSVFile(ip2LocationCSVFile)
	.AddGeographicAreasFromGeoJSONFile(geoJSONDemoFilePath)
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
	.AddDefaultAction((ip) => Console.WriteLine($"The IP Address: {ip} is outside all the areas provided"))
	.Build();
```
## Running the engine
```csharp
//Billings, MT IP Address
engine.Run("98.127.147.57");

//New York, NY IP Address
engine.Run("172.254.112.210");
```
