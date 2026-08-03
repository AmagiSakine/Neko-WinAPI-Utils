#include "NekoSystemUtils.h"
#include <Windows.h>
#include <TlHelp32.h>
#include <string_view>

// A local RAII wrapper for safely managing system HANDLES.
// Ensures that CloseHandle is automatically called when the variable goes out of scope.
struct FScopeHandle
{
    HANDLE Handle;
    explicit FScopeHandle(HANDLE h) : Handle(h) {}
    ~FScopeHandle() { if (IsValid()) CloseHandle(Handle); }

    operator HANDLE() const { return Handle; }
    bool IsValid() const { return Handle != nullptr && Handle != INVALID_HANDLE_VALUE; }
};

std::wstring NekoSystemUtils::GetActiveWindowTitle()
{
    HWND hWindow = GetForegroundWindow();
    if (!hWindow) return std::wstring();

    const int BufferSize = 256;
    wchar_t Buffer[BufferSize];
    int Length = GetWindowTextW(hWindow, Buffer, BufferSize);

    if (Length > 0) return std::wstring(Buffer, Length);
    return std::wstring(L"Unknown");
}

std::vector<FNekoProcessInfo> NekoSystemUtils::GetAllProcesses()
{
    std::vector<FNekoProcessInfo> ProcessList;

    // Safely capture process snapshots
    FScopeHandle hSnapshot(CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0));
    if (!hSnapshot.IsValid()) return ProcessList;

    PROCESSENTRY32W pe32;
    pe32.dwSize = sizeof(PROCESSENTRY32W);

    if (Process32FirstW(hSnapshot, &pe32))
    {
        do {
            FNekoProcessInfo Info;
            Info.ProcessID = pe32.th32ProcessID;
            Info.ProcessName = pe32.szExeFile;
            ProcessList.push_back(Info);
        } while (Process32NextW(hSnapshot, &pe32));
    }

    return ProcessList;
}

bool NekoSystemUtils::KillProcessByName(const std::wstring& TargetName)
{
    FScopeHandle hSnapshot(CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0));
    if (!hSnapshot.IsValid()) return false;

    PROCESSENTRY32W pe32;
    pe32.dwSize = sizeof(PROCESSENTRY32W);
    bool bKilled = false;

    if (Process32FirstW(hSnapshot, &pe32))
    {
        do {
            // Using string_view prevents unnecessary memory allocations in the loop
            std::wstring_view CurrentName(pe32.szExeFile);

            if (CurrentName.find(TargetName) != std::wstring_view::npos)
            {
                FScopeHandle hProcess(OpenProcess(PROCESS_TERMINATE, FALSE, pe32.th32ProcessID));
                if (hProcess.IsValid())
                {
                    TerminateProcess(hProcess, 0);
                    bKilled = true;
                }
            }
        } while (Process32NextW(hSnapshot, &pe32));
    }

    return bKilled;
}

void NekoSystemUtils::SimulateKeyPress(int VirtualKey)
{
    INPUT ip = { 0 };

    ip.type = INPUT_KEYBOARD;
    ip.ki.wVk = VirtualKey;
    ip.ki.wScan = 0;
    ip.ki.time = 0;
    ip.ki.dwExtraInfo = 0;

    // The KEYEVENTF_EXTENDEDKEY flag is strictly required. Without it, Windows ignores
    // hardware synthesis of specific keys (e.g., Volume Mute or Media Next).
    ip.ki.dwFlags = KEYEVENTF_EXTENDEDKEY;
    SendInput(1, &ip, sizeof(INPUT));

    ip.ki.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_EXTENDEDKEY;
    SendInput(1, &ip, sizeof(INPUT));
}