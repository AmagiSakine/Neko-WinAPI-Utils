\# N.E.K.O. - OS Interaction Module (Win32 API)



\*\*Description:\*\*

This repository contains an isolated `C++` module extracted from my larger R\&D project (N.E.K.O.). The core project is a distributed client-server AI architecture. This specific repository demonstrates the low-level Windows API interactions required for the system's OS-level awareness.



\*\*Tech Stack:\*\*

\* C++ (Standard 17+)

\* Win32 API

\* Visual Studio



\*\*Key Features:\*\*

\* \*\*Window Tracking:\*\* Captures active foreground windows dynamically.

\* \*\*Process Management:\*\* Enumerates running OS processes via `CreateToolhelp32Snapshot` (implemented with RAII for memory safety and leak prevention).

\* \*\*Input Synthesis:\*\* Simulates hardware-level keystrokes using `SendInput`.



\*\*Note:\*\* This repository serves as a standalone proof-of-concept demonstrating system-level C++ programming and safe WinAPI usage. The full distributed architecture remains private.

