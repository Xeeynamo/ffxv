# Reverse engineering of Final Fantasy XV (PC, PS4, Xbox One)

## Format supported

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
PS4 texture format. Not completed at all. Everything is currently commented.

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

Convert all the ebex files in the game folder to XML

```
dotnet xmbtool -i ./ffxv-epduscae ./ffxv-epduscae-out -x -d -r
```
