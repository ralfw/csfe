# CSFE Demo
The C# solution show how CSFE can be used. Open the `charcount.sln` file in Visual Studio oder Rider.

When you compile it a directory named `flow/` will be generated. It contains the flow definition and all services.

Open a terminal window on `flow/` and run this command:

`mono process.exe -s=source.csfe -i=input`

(On windows you can omit `mono`).

The flow will process an input file in `input/` and produce an output file in `output/`.

Enjoy!