#include "NekoSystemUtils.h"
#include <iostream>
#include <Windows.h>

int main()
{
    // Set console encoding to UTF-8 to correctly display Unicode window titles
    SetConsoleOutputCP(CP_UTF8);

    std::wcout << L"=== N.E.K.O. System Utils Test ===" << std::endl;

    // 1. Test Window Tracking
    std::wstring activeWindow = NekoSystemUtils::GetActiveWindowTitle();
    std::wcout << L"[TEST 1] Active Window: " << activeWindow << std::endl;

    // 2. Test Process Enumeration
    std::vector<FNekoProcessInfo> processes = NekoSystemUtils::GetAllProcesses();
    std::wcout << L"[TEST 2] Total Processes Found: " << processes.size() << std::endl;

    // Print first 5 processes just to verify data
    std::wcout << L"Listing first 5 processes:" << std::endl;
    for (size_t i = 0; i < processes.size() && i < 5; ++i)
    {
        std::wcout << L"  PID: " << processes[i].ProcessID
            << L" | Name: " << processes[i].ProcessName << std::endl;
    }

    std::wcout << L"==================================" << std::endl;

    // Pause console so it doesn't close immediately
    std::system("pause");
    return 0;
}