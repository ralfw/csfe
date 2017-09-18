# CSFE - Console Service Flow Engine
With the CSFE you can integrate several console programs into a data processing flow.

## Introduction

* _Console program_: A program that has no "real" UI - no web-UI, no GUI, not even a REST API. It's soley driven by data from the command line and input files and producing data as output files. It normally runs in a terminal window.
* _Data (processing) flow_: A problem is solved by letting input data flow through a sequence operations which successively transform into the desired output.

Here's a simple example:

> Count the number of occurrences of each character in a string; do not distinguish between upper and lower case. For the string "HelLo" the result would be [('h',1),('e',1),('l',2),('o')].

And here's an approach to how to solve the problem:

1. Split the input string into its characters.
2. Normalize the characters, i.e. make them all lower case.
3. For each character count its occurrenes and produce the output.

Written as a _data flow_ it could look like this:

`split > normalize > count`

(If you like, replace `>` in your mind with the UNIX `|` operator.)

Now, with the CSFE this data flow actually is executable. You just have to implement the operations `split`, `normalize`, and `count` as _console programs_.

You can use any programming language/stack you like to implement CSFE operations. Use Java or C# or Go or Ruby or even a shell script language. Hence the console programs are called _services_: they are providing just a small service within a larger solution. And they adere to a platform independent contract. Communication between services works by "sending" files in the direction of the `>` operator.

To get the data flow solution up and running the operations need to be coded in a certain way. The basic CSFE service contract is:

* Read input from files in the `/input` directory.
* Write output to files in the `/output` directory.

When deployed all operations reside in their own directories off the data flow root directory. The directory structure of a CSFE solution looks like this:

```
/charcounter
  /input <- input to flow goes here
  /output <- output of flow is put here
  
  /split <- service directory
    /input
    /output
  /normalize
    /input <- service reads input from here
    /output 
  /count
    /input
    /output <- service writes output here
```

When pointed to the flow directory `/charcounter` the CSFE crawls the directory tree to find all services. For that it requires them to identify themselves with a small meta-data file `service.json` in their service directory alongside its executable.

```
/split
  service.json <- meta-data
  split.exe <- one or more files for the service itself
  /input
  /output

```

Executing the data flow means, files are pushed from the flow input to the first service's local input. Then the service's executable is run and writes its results to its local output directory. From there the flow engine pulls the files and pushes them to the next service in the flow. And so on, and so forth.

The output of the final operation in the data flow will end up in the flow's own output directory.

## The rational behind CSFE
You might be asking yourself, why not use the operating system's own pipe operator `|` to connect services in a data flow?

There are several reasons why I think a CSFE would be handy in addition to that:

* The `|` only works with standard input/output streams. They are easy to use, but limited. All input has to go through them in a linear fashion. With input/output coming from files data access has more degrees of freedom.
* There is no branching when letting data flow with `|`. Data flows are limited to very simple one-dimensional sequences of operations. That's making it hard to solve some problems with data flows.
* Using the full power of the UNIX pipe operator requires developers to be comfortable with shell programming. Not all devs know the shell that well, though. (Myself included.)

Of course such data processing by running a bunch of programs after oneanother will not have great performance. So why do it in the first place whether with `|` or the CSFE?

To me it's a tool I use in Clean Code developer trainings:

* With the CSFE I can split up a solution into service with very simple, very clear cut contracts and have them developed in parallel by groups of training participants. That's an exercise in contract-first programming as well as distributed development. It greatly helps to think in terms of modularization.
* With the CSFE I'm not only able to integrate several services into a whole solution, I also can integrate developers on different platforms into a team. CSFE allows polyglot solutions. Having developers with different backgrounds in a training no longer is a difficulty but an opportunity when doing programming exercises. 
* Getting training participants to work as a team on a modular code base makes it possible to tackle larger exercises. Trainings thus become more fun.