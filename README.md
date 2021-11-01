# ﻿Csv Deserializer library

This project contains modern, fast, highly configurable, low allocation Csv Reader for .Net applications.

Library has low level CsvReader class, that can read csv nodes one by one and highly configurable high level CsvDeserializer class able to deserialize csv to IEnumerable of poco objects (plain old CLR objects).

You can download it as NuGet package from [NuGet Gallery | CsvDeserializer](https://www.nuget.org/packages/CsvDeserializer/).

## How to use it

In `WojciechMikołajewicz.CsvReader` namespace exists `CsvDeserializer<TRecord>` class where TRecord is a type of objects you want to deserialize csv to. To read a csv file you need to:

```c#
using System.IO;
using WojciechMikołajewicz.CsvReader;

using var textReader = new StreamReader(pathToCsvFile, Encoding.UTF8, true);//Create TextReader
using var csvDeserializer = new CsvDeserializer<Record>(textReader);//Create CsvDeserializer from TextReader

//Read csv records (lines) to Record objects
await foreach(var item in csvDeserializer.ReadAsync().WithCancellation(cancellationToken).ConfigureAwait(false))
{
    //Do something with the item
}
```

Above is the simplest case. It assumes that csv file has a header row and that Record properties are named same as columns in header row in csv file. It also assumes standard delimiter which is coma ',' as name of the format says (csv – comma-separated values).

### Change settings

To change default settings like column separator or others you have to pass `CsvDeserializerOptions` object to `CsvDeserializer` constructor like that:

```c#
using var csvDeserializer = new CsvDeserializer<Record>(textReader, new CsvDeserializerOptions()
{
	DelimiterChar = '\t',//Changes columns separator to tab
});
```

You can also change deserialization culture (default is `CultureInfo.InvariantCulture`), set absence of header row, turn off string deduplication and tune other settings.

### Configure record deserialization

If your csv file doesn't contain header row or column names in header row are different than property names of your Record class or you want to change other aspects of deserialization to Record class, you should provide record configuration object.

Assume your record class looks like this:

```c#
public class Person
{
    public string Name { get; set; }
    
    public DateTime DateOfBirth { get; set; }
}
```

To configure this `Person` class deserialization process you have to create configuration class that implements `ICsvRecordTypeConfiguration<Person>` interface – example below:

```c#
class PersonConfiguration : ICsvRecordTypeConfiguration<Person>
{
	public void Configure(RecordConfiguration<Person> recordConfiguration)
	{
		recordConfiguration.Property(prsn => prsn.Name)//Get Name property
			.BindToColumn(0);//Bind it to first column in csv file (index is zero based)
        
		recordConfiguration.Property(prsn => prsn.DateOfBirth)//Get DateOfBirth property
			.BindToColumn(1)//Bind it to second column in csv file
			.ConfigureDeserializer(dsrl =>//Configure DateTime deserializer
			{
				dsrl.SetFormat("yyyyMMdd");//Set some fancy date format
			});
       }
}
```

Then you have to pass `PersonConfiguration` object to `CsvDeserializer` constructor:

```c#
using var csvDeserializer = new CsvDeserializer<Person>(textReader, new CsvDeserializerOptions()
{
	DelimiterChar = '\t',//Changes columns separator to tab
	HasHeaderRow = false,//File does not contain header row
}, new PersonConfiguration());//Creates and pass person configuration object to constructor
```

And that's it. You can now enumerate results from `ReadAsync` or `Read` methods (like in first example).