# Proteus
A growing library of data processing functions.

# Cabeiro
A command line tool that exposes Proteus functionality.

## History

Around 2007, I was working on a project that involved analyzing log files and I found that I often needed to perform various simple queries on these files.
At first, I wrote some scripts to perform these queries, but then, as the number of operations increased, I decided to write a command line tool that I named *LogTools*.
Over the next decade, I kept adding functionality to LogTools.

The philosophy of the tool was to support very simple operations, each of which was easy to understand.
But these simple operations were meant to support more complex queries by being chained.
And by having the result of each operation saved within its own file, one could always go back to try a different set of queries on an intermediate data set.
I am calling this process *interactive analysis* of a data file.

In 2019, I decided to start writing from scratch a new open-source version: *Cabeiro*.
Unlike LogTools, which was written in a hurry over a long period of time and which included lots of duplicated code pieces, Cabeiro is meant to identify processing patterns, so as to share as much code as possible between similar operations.
Thus, Cabeiro means to accomplish all that LogTools did, but also, to easily permit the addition of new functionality, by simply extending existing processing patterns.

## General command syntax:

```Cabeiro <command_name> [<command_arguments>]```

Useful notes:

 1. Command names and arguments are case-insensitive.
 2. Errors, warnings, and progress messages are always printed to standard error (you can redirect these using '2>').
 3. The output file will be overwritten if it already exists.
 4. A '.' is printed for each 1,000 rows, an 'o' for each 100,000, and an 'O' for each 1,000,000.
 5. The string 'tab' can be used on the command line if you need to indicate a tab character.
 6. Row and column numbers start from 1.

## Full list of Cabeiro commands:

For detailed instructions on each specific command, execute:

```Cabeiro help <command_name>```

#### HELP commands:

* HELP - Obtain help on Cabeiro functionality.

#### INFORMATION commands:

* C - (C)ount lines
* AL - (A)nalyze (L)ines
* ACV - (A)nalyze (C)olumn (V)alues

#### EDITING commands:

* EL - (E)dit (L)ines
* ECV - (E)dit (C)olumn (V)alues

#### SELECTION commands:

* SLHCV - (S)elect (L)ines (H)aving (C)olumn (V)alue
* SLBLN - (S)elect (L)ines (B)y (L)ine (N)umber
