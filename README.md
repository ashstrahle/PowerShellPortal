# PowerShell Portal

## This project has been superceded. Please visit https://github.com/ashstrahle/PowerShellBlazor

Execute PowerShell scripts via .Net WebApp with real-time output

Useful for automation and user accessibility, this is a working .NET WebApp that asynchronously executes PowerShell scripts, and writes the Output, Progress, Warning, and Error streams to a results window in real-time.

**test.ps1** (included in this project):
```powershell
for ($i=1; $i -le 5; $i++) {
    Write-Progress "Loop $i - progress output (Write-Progress)"
    Write-Output "Normal output text (Write-Output)"
    Write-Warning "Here's some warning text (Write-Warning)"
    Write-Error "Oh no, here's some error text (Write-Error)"
    Start-Sleep -s 1
}
```
Looks like this...

![Results](Images/Results.gif)

## Prerequisites

* Windows authentication enabled, and anonymous authenticated disabled. This is because SignalR is used to provide live feedback of the running script - as such, a SignalR group is created using the username associated with each session. If set correctly, you should see your username in the top right of the initial form.

* PowerShell scripts must be non-interactive; there's no means to provide input back to PowerShell scripts once they're running.

* Output must be written using Write-Output, Write-Progress, Write-Warning, or Write-Error only. Output written with Write-Host cannot be captured and hence won't display in the results window.

* Ensure the PowerShell Execution Policy has been sufficiently opened to allow your scripts to run. If in doubt and at own risk, as Administrator run:
```powershell
    Set-ExecutionPolicy Unrestricted -Scope CurrentUser
    Set-ExecutionPolicy Unrestricted -Scope Process
```
Note: for Visual Studio users, execute the above in Package Manager Console.
    
* Place your PowerShell scripts in **~/Scripts/**. .NET requires the path to be relative to the project, hence this location.

## Author

* **Ashley Strahle** - [AshStrahle](https://github.com/AshStrahle)
