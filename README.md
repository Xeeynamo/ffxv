# Reverse engineering of Final Fantasy XV (PC, PS4, Xbox One)

## Format supported
### XMB
XMB is used by Final Fantasy XV Episode Duscae and Platinum Demo
[Full documentation](https://www.lucianociccariello.com/research/finalfantasyxv#xmb)
![Alt text](docs/sample-ctrl_cmn_text_icon.png?raw=true "ctrl_cmn_text_icon sample")

### XMB2
XMB2 is used by the final version of Final Fantasy XV and the Judgment Demo.
It has not been reversed yet.

### EARC
A file archive format that it is responsable to link other dependent EARC files.
Not the best implementation here, but it works. I recommend to use FFXV Scout or Noesis.

### BTEX
PS4 texture format. Not completed at all. Everything is currently commented.

## Tools
### FFXV
Used mostly for internal testing.
* Xmb.EvaluateUntouched() checks for all the properties that has not been evaluated ('touched') and that are orphan to the XMB library.
* Xmb.Touch() marks the tables as 'touched', without relying to Xmb.GetVariables() for that.
* Xmb.EvaluateUnknownTypes() checks for types not supported by the library (type 6 and 7 are missing for example)