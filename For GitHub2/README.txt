# Dual Personality PoC

Simple proof-of-concept of two independent AI agents with separate persistent memory.

## What it does

- Two different personalities (Fubuki and Sameko Saba)
- Each personality has its own chat history stored locally as JSON
- You can switch between them at any time
- Basic desktop chat interface (WPF)

## Important notes

This is **not** a production application.

It was originally built as a personal experiment to test the idea of multiple agents with isolated memory.

The public version has been cleaned up:

- Heavy local LLM (Gemma) was replaced with a simple stub
- External services (Telegram, Google Search) were removed
- Only the core architecture remains

The original version used LLamaSharp + CUDA + a local 9B model.

## How to run

1. Open the solution in Visual Studio 2022
2. Restore NuGet packages
3. Build and run

No additional model files are required for the current stub version.

## Project structure

- `MainWindow.xaml` / `MainWindow.xaml.cs` — UI and main logic
- `NekoBrain` — agent logic (currently a stub)
- Memory files are saved in the Documents folder:
  - `Fubuki_history.json`
  - `SABA_history.json`

## Status

Personal R&D / learning project.  
Code quality was secondary to testing the concept.