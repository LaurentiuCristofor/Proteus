# Proteus
A growing library of data processing functions.

# Cabeiro
A command line tool that exposes Proteus functionality.

## History

Around 2007, I was working on a project that involved analyzing log files and I found that I often needed to perform various simple filtering and editing operations on these files.
At first, I wrote some scripts to perform these operations, but then, as their number increased, I decided to write a command line tool that I named *LogTools*.
Over the next decade, I kept implementing more operations in LogTools, such that it grew to support many different filtering and editing operations.

The philosophy of the tool was to support very simple operations, such that each of them would be easy to understand and use, but these simple operations could also be chained to support arbitrarily complex ones.
And by having the result of each operation saved within its own file, one could always go back to trying a different set of operations starting from an intermediate data set.
When used for data analysis, this allowed performing what I call ""interactive analysis"" of a data file.
 
In 2019, I decided to start writing from scratch a new open-source version: *Cabeiro*.
I also decided to separate the core file processing operations from the command line tool, so that's how the *Proteus* library came into being.
Unlike LogTools, which was written piece by piece in a hurry over a long period of time, and which included lots of duplicated code pieces, Cabeiro is meant to identify processing patterns, so as to share as much code as possible between similar operations.
Cabeiro also attempts to avoid providing duplicated functionality; where LogTools sometimes offered special-cased versions of certain functions, Cabeiro will attempt to provide only the more general functionality.
Thus, Cabeiro means to accomplish all that LogTools did, but also, to easily permit the addition of new functionality, by simply extending existing processing patterns.

If you work with log data or with any type of text data stored in a tabular format, you may find Cabeiro to be a very useful tool.

## Getting started with Cabeiro

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

#### HELP commands

* HELP - Obtain help on Cabeiro functionality.

#### INFORMATION commands

* C - (C)ount lines
* AL - (A)nalyze (L)ines
* ACV - (A)nalyze (C)olumn (V)alues

#### ORDERING commands

* I - (I)nvert file
* S - (S)ort file
* SBCV - (S)ort file (B)y (C)olumn (V)alue

#### EDITING commands

* EL - (E)dit (L)ines
* ECV - (E)dit (C)olumn (V)alues
* IL - (I)nsert (L)ine

#### SELECTION commands

* SLBCV - (S)elect (L)ines (B)y (C)olumn (V)alue
* SLBLN - (S)elect (L)ines (B)y (L)ine (N)umber
* SCBCN - (S)elect (C)olumns (B)y (C)olumn (N)umber
* SLBLS - (S)elect (L)ines (B)y (L)ine (S)tring
* SLBCS - (S)elect (L)ines (B)y (C)olumn (S)tring

#### SPLITTING commands

 * SPLR - (SP)lit (L)ine (R)anges
 * SPC - (SP)plit (C)olumns
 * SPCV - (SP)plit (C)olumn (V)alues

#### POST_SORTING commands

* SB2CV - (S)ort file (B)y (2)nd (C)olumn (V)alue
