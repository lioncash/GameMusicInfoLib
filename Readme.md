# Game Music Info Library

## What is it?

GameMusicInfoLib is a C# library that can be used for retrieving info from game music file formats (ie, NSF, SPC, SAP, etc).
It also supports reading several module files.

## How do I use it?

It should be fairly simple to use. For the format you wish to read simply do the following.

For example, say I want to read a Nintendo NSF file:

    NsfReader nsf = new NsfReader(/* path to the NSF file here */);

Then you can use the methods within that reader to get info from that type of file.


