cd Milefa-Webserver\bin\Release\netcoreapp2.2\linux-arm\publish\
mkdir tmp
copy * tmp\
del tmp\appsettings.json
pscp.exe -r .\tmp\* pi@10.16.1.12:/home/pi/milefa
del /F /Q tmp\
pause