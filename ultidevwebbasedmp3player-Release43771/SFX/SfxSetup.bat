@echo off
setup.exe
rem now wait 20 seconds
@ping 127.0.0.1 -n 21 -w 1000 >nul
