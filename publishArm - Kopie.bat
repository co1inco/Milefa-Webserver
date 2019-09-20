cd Milefa-Webserver\bin\Release\netcoreapp2.2\publish\
mkdir tmp
copy * tmp\
del tmp\appsettings.json
pscp.exe -r .\tmp\* pi@192.168.2.16:/home/pi/milefa
del /F /Q tmp\
pause