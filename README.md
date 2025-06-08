#  File Content Indexing System (C# Final Project)

## Project Overview

This is a distributed system built with C# that indexes the contents of `.txt` files using **two agent applications** and a **master application**.  
Each agent scans text files, counts word occurrences, and sends the results to the master process using **named pipes**.



##  Components

###  ScannerA
- Reads `.txt` files from a user-provided directory.
- Counts word occurrences per file.
- Sends the data to the master via the named pipe `agentA`.

###  ScannerB
- Same as ScannerA, but connects via the named pipe `agentB`.

### Master
- Receives word count data from both agents concurrently.
- Merges and displays the combined word count results.
- Uses multithreading to handle each pipe.
- Runs on a dedicated CPU core for performance.

---

##  Technologies Used

- **C# / .NET 8.0**
- **Console Applications**
- **Named Pipes** (`NamedPipeServerStream`, `NamedPipeClientStream`)
- **Multithreading** (`Thread`)
- **Processor Affinity** (`Processor.ProcessorAffinity`)
- **JSON Serialization** (`System.Text.Json`)

---

##  How to Run

###  Build the Solution
Open terminal in each folder (Master, ScannerA, ScannerB) and run:
```bash
dotnet build
