# Reverse engineering of Final Fantasy XV (PC, PS4, Xbox One)

## How to contribute

### Package models

Not all the packages have all the types mapped. Creating a model is straight forward.

1) Dump your copy of Episode Duscae or Platinum Demo from your Playstation 4 (go [here](https://www.lucianociccariello.com/research/finalfantasyxv?ref-github) and press F12)
2) From the dump, extract every EARC content with [Noesis](https://richwhitehouse.com/index.php?content=inc_projects.php&showproject=91).
3) Convert all the EXML files to XML using [xmbtool](#xmbtool).
4) Put the XML file into FFXV.Services.Tests binary folder.
5) Add the test and run it.
6) When you receive an exception like 'Type SQEX.Ebony.Framework.Sequence.Tray.SequenceTray not found', just create the model in the respective project and write the implementation looking the XML structure. The PackageService will automatically recognize it.
7) Run the test and be 100% sure that it passes.
8) Create a pull request :)

Currently XmbAcceptanceTest and XmbEqualityTest passes, which guarantee that the XMB de/encoder is working fine! 

### XML to XMB performance

Currently there is a performance issue converting from XML to XMB1, where equal elements are written more than once instead of using the ElementIndexTable to make the file smaller. The converted EXML are still readable from the game, but performance problems could arise (to confirm).

The tests XmbOptimizationTest and XmbPerformanceTest will fail due to this mis-optimization of the XMB encoder.

### Tools

The following tools are missing:

* Convert between XML and XMB2 (PS4 and PC version of the final game).
* A save editor (I have a prototype that I will upload later).
* BTEX converter (there is the need to convert between PS4 and PC textures).
* Package editor (to create quests, debug menu, enemies, dungeons, expaing maps).

## Format supported

### Package
The main boss of the game. Every EXML/XMB/XMB2 can be converted to an XML, which the Luminous Engine reads that as 'package'.

A package is a descriptor that drives almost all the aspects of the game. On FFXV.Services there is a PackageService which de-serialize an XML and returns a Package object.

A package contains a list of objects, where every object is part of a group of nodes connected to another object (imagine that as a tree).

Every object has a type, which is used by PackageService by just creating its model into the projects Black, SQEX.Ebony or SQEX.Luminous.

Since the complexity of the PackageService deserializer, there are some tests to validate the way which it behaves.

### XMB
XMB is used by Final Fantasy XV Episode Duscae and Platinum Demo and can be modified using [xmbtool](#xmbtool).

A lot of source-code currently has assertions and checks, that will be removed once the library is complete.

The missing step to convert from XML to XMB is that I am not re-using the existing elements, generatig a huge file. For example the original debug_wm.exml is only 757KB but the converted one is 2.1MB! It works on the retail game, but repacking everything will probably lead to memory issues in the game. I plan to optimize later. It is better than nothing, right?

[The full documentation is available here.](https://www.lucianociccariello.com/research/finalfantasyxv#xmb)

### XMB2
XMB2 is used by the final version of Final Fantasy XV and the Judgment Demo.
It has not been reversed yet.

### EARC
A file archive format that it is responsable to link other dependent EARC files.
Not the best implementation here, but it works. I recommend to use FFXV Scout or Noesis.

### BTEX
PC/PS4 texture format. Not completed at all. Everything is currently commented.

## Tools

All the command-line tools requires [.NET Core runtime](https://www.microsoft.com/net/download) installed and runs on Windows, Linux and macOS.

All the graphical tools requires [.NET Framework runtime](https://www.microsoft.com/net/download) and runs on Windows only.

### xmbtool

Converts between XMB and XML.

**Parameters**

`-i|--input` Input file or directory.

`-o|--output` Output file or directory.

`-x|--export` If the tool should convert from XMB to XML. If not specified, it will convert from XML to XMB.

`-d|--directory` The input and output will be considered as directories instead of files.

`-r|--recursive` Recursively process all the files from the input directory to the output directory.

**Examples**

Convert a XMB file to XML:

```
dotnet xmbtool -i layout_title_epd.exml -o layout_title_epd.xml -x
```

Convert a XML back to XMB:

```
dotnet xmbtool -i layout_title_epd.exml -o layout_title_epd.xml
```

Convert all the exml files in the game folder to XML

```
dotnet xmbtool -i ./ffxv-epduscae ./ffxv-epduscae-out -x -d -r
```
