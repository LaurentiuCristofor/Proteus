#!/usr/bin/env bash

# Ensure that output folder exists.
#
mkdir -p build

# Build Proteus.
#
mcs -target:library -out:./build/Proteus.dll -recurse:../Proteus/*.cs Proteus.AssemblyInfo.cs

# Build Cabeiro.
#
mcs -target:exe -out:./build/Cabeiro.exe -recurse:../Cabeiro/*.cs -reference:./build/Proteus.dll Cabeiro.AssemblyInfo.cs
