dotnet publish -c Release -r win-x64 ./FFXV.Tools.Xmb -o ../bin/win
dotnet publish -c Release -r linux-x64 ./FFXV.Tools.Xmb -o ../bin/osx
dotnet publish -c Release -r osx-x64 ./FFXV.Tools.Xmb -o ../bin/linux
