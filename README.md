# Proteus
A growing library of data processing functions.

# Cabeiro
A command line tool that exposes Proteus functionality for data investigation and data processing.

## Overview

The primary goal of Cabeiro is to help with the analysis of text data organized in a table format; i.e. each line consists of a set of column values delimited by some form of column separator.
In this scenario, the user doesn''t know exactly what they are looking for, so they need to execute various probing queries, which will guide their next operations.
Cabeiro offers a large variety of data selection approaches that can help in such process.
However, due to the atomic nature of the operations provided, Cabeiro is not suitable for performing complex data processing operations in a production environment; in such scenario, a custom data processing tool would be required and such tool could be written on top of the Proteus library.

A secondary goal of Cabeiro is to provide a set of text file editing operations that can be used for manipulating text data or for cleaning up a data file.

Thus, Cabeiro can help with data analysis and data cleanup operations, while the Proteus library can be reused to create custom tools when performance is needed for executing more complex data processing operations.

### Performance notes

Most of the Cabeiro operations scan an input file, so their performance is linear in terms of the size of the input file.
The memory use of each command varies according to its type of processing and is mentioned in the online help description of the command.
Most commands only need to access a single line of data at a time, but some commands need access to the entire file''s content.

If you operate on very large files and you don''t have enough memory to hold all their data, then some operations may run out of memory.
Some of Cabeiro''s operations are meant to help processing files that are too large to hold in memory.
For example, you can break a file into smaller chunks, sort them, then merge them back into one large sorted file.
Some memory-expensive operations also have less-expensive alternatives that work on sorted files.

### Design notes

Cabeiro is meant to support only simple and generic data processing operations.
The Proteus library could grow to support more complex data analysis scenarios, which would then be exposed by different tools than Cabeiro. 

## Build instructions

On Windows systems, use Visual Studio with `Proteus\Proteus.sln` and `Cabeiro\Cabeiro.sln`.

On Linux systems, use Mono and the `Mono/build.sh` script.
Once `Cabeiro.exe` is built under `Mono/build`, it can be executed with `mono Cabeiro.exe`.

## Getting started with Cabeiro

Most of the information in this section is also available through Cabeiro''s help function.
The command names are a compromise between being suggestive and being short enough to insert in the automatically-generated names of the output files, thus offering a way to document the content of an output file through its name.

### General command syntax

```Cabeiro <command_name> [<command_arguments>]```

Useful notes:

 1. Command names and arguments are case-insensitive.
 2. Errors, warnings, and progress messages are always printed to standard error (you can redirect these using '2>').
 3. The output file will be overwritten if it already exists.
 4. A '.' is printed for each 1,000 rows, an 'o' for each 100,000, and an 'O' for each 1,000,000.
 5. The string 'tab' can be used on the command line if you need to indicate a tab character.
 6. Row and column numbers start from 1.

### Full list of Cabeiro commands:

For detailed instructions on each specific command, execute:

```Cabeiro help <command_name>```

Commands operating on 'column values' interpret columns as values of a specific data type.
Commands operating on 'column strings' interpret columns as plain strings.
This distinction is important because, for example, `0` and `0.0` would be seen as the same value if they are interpreted as floating point data, but they will be seen as different values if they are interpreted as plain strings.

#### HELP commands

* HELP - Obtain help on Cabeiro functionality.

#### INFORMATION commands

These commands report information about the content of a file.

* C - (C)ount lines
* AL - (A)nalyze (L)ines
* ACV - (A)nalyze (C)olumn (V)alues

#### ORDERING commands

These commands change the order of the lines or columns of a file.

* I - (I)nvert file
* S - (S)ort file
* SBCV - (S)ort file (B)y (C)olumn (V)alue
* SH - (SH)uffle file
* OC - (O)rder (C)olumns

#### EDITING commands

These commands modify the content of a file.
You can edit data, insert new data, or combine data from two files.

* EL - (E)dit (L)ines
* ECS - (E)dit (C)olumn (S)trings
* ECV - (E)dit (C)olumn (V)alues
* IL - (I)nsert (L)ine
* JL - (J)oin (L)ines
* CL - (C)oncatenate (L)ines
* TC - (T)ransform (C)olumns

#### SELECTION commands

These commands extract a subset of a file, according to various criteria.
To eliminate some lines, just select the ones you want to keep.
To handle duplicate values, use the "(R)elative to other lines" selection commands.

* SLBCV - (S)elect (L)ines (B)y (C)olumn (V)alue
* SLBN - (S)elect (L)ines (B)y (N)umber
* SCBN - (S)elect (C)olumns (B)y (N)umber
* SLBLS - (S)elect (L)ines (B)y (L)ine (S)tring
* SLBCS - (S)elect (L)ines (B)y (C)olumn (S)tring
* SLBCC - (S)elect (L)ines (B)y (C)olumn (C)ount
* SLBLSR - (S)elect (L)ines (B)y (L)ine (S)tring (R)elative to other lines
* SLBCVR - (S)elect (L)ines (B)y (C)olumn (V)alue (R)elative to other lines
* SLBLIF - (S)elect (L)ines (B)y (L)ookup (I)n (F)ile
* SLBCVLIF - (S)elect (L)ines (B)y (C)olumn (V)alue (L)ookup (I)n (F)ile
* SLS - (S)elect (L)ines (S)ample

#### SPLITTING commands

These commands split the content of a file across multiple files, based on various criteria.

 * SPLR - (SP)lit (L)ine (R)anges
 * SPC - (SP)plit (C)olumns
 * SPCV - (SP)plit (C)olumn (V)alues

#### POST_SORTING commands

These commands expect that their input files had been previously sorted; column value variants expect files to be sorted on the respective column.
Some of these commands provide memory-efficient alternatives to commands from other categories.

* SB2CV - (S)ort file (B)y (2)nd (C)olumn (V)alue
* ML - (M)erge (L)ines
* MLBCV - (M)erge (L)ines (B)y (C)olumn (V)alue
* SLPSBLSR - (S)elect (L)ines (P)ost (S)orting (B)y (L)ine (S)tring (R)elative to other lines
* SLPSBCVR - (S)elect (L)ines (P)ost (S)orting (B)y (C)olumn (V)alue (R)elative to other lines
* SLPSBLIF - (S)elect (L)ines (P)ost (S)orting (B)y (L)ookup (I)n (F)ile
* SLPSBCVLIF - (S)elect (L)ines (P)ost (S)orting (B)y (C)olumn (V)alue (L)ookup (I)n (F)ile
* JLPS - (J)oin (L)ines (P)ost (S)orting
* FST - (F)ind (S)tate (T)ransitions

#### GENERATION commands

These commands generate data.

* GEND - (GEN)erate (D)istribution
* GENS - (GEN)erate (S)ample

## History

Around 2007, I was working on a project that involved analyzing log files and I found that I often needed to perform various simple filtering and editing operations on these files.
At first, I wrote some scripts to help me perform these operations, but then, as their number increased, I decided to write a command line tool that I named *LogTools*.
Over the next decade, I kept adding more functionality to LogTools, such that it grew to support many different filtering and editing operations.

The philosophy of the tool was to support very simple operations, such that each of them would be easy to understand and use, but these simple operations could also be chained to support arbitrarily complex ones.
And by having the result of each operation saved within its own file, I could always go back to trying a different set of operations starting from an intermediate data set.
When used for data analysis, this allowed me to perform what I call an ""interactive analysis"" of a data file.
 
In 2019, I decided to start writing from scratch a new open-source version of LogTools and name it *Cabeiro*.
I also decided to separate the core file processing operations from the command line tool, so that's how the *Proteus* library came into being.
LogTools had been written piece by piece over a long period of time, often, by copying the code of an operation and then editing it to perform a new one.
This approach resulted in a lot of duplication of its processing patterns, despite some efforts to factor common processing in generic functions.
With Cabeiro I set out to identify these processing patterns, so as to share as much code as possible between similar operations.
Cabeiro also attempts to avoid providing duplicated functionality; where LogTools sometimes offered special-cased versions of certain functions, Cabeiro attempts to provide only the more general functionality.
Thus, Cabeiro and Proteus mean to accomplish all that LogTools did, but also, to easily permit the addition of new functionality, by simply extending existing processing patterns.
