#pragma once
#include <string>
#include <vector>

/// @brief Data structure representing active OS process information.
struct FNekoProcessInfo
{
    int ProcessID;
    std::wstring ProcessName;
    std::wstring WindowTitle;
};

/// @brief Utility class for interacting with Windows OS internal APIs.
class NekoSystemUtils
{
public:
    /// @brief Retrieves the title of the currently focused foreground window.
    /// @return A wide string containing the window title, or an empty string if it fails.
    static std::wstring GetActiveWindowTitle();

    /// @brief Captures a snapshot of all currently running processes in the OS.
    /// @return A vector containing process IDs and executable names.
    static std::vector<FNekoProcessInfo> GetAllProcesses();

    /// @brief Terminates a process by matching its executable name.
    /// @param ProcessName The wide string name of the target process (e.g., L"notepad.exe").
    /// @return True if the process was found and terminated successfully, otherwise false.
    static bool KillProcessByName(const std::wstring& ProcessName);

    /// @brief Synthesizes keystrokes, virtual keyboard events, and hardware interrupts.
    /// @param VirtualKey The virtual-key code (VK) to simulate.
    static void SimulateKeyPress(int VirtualKey);
};

