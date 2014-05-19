copy passportDesu.jpg c:\passportDesu.jpg
copy image1.jpg c:\image1.jpg
copy image2.jpg c:\image2.jpg
sqlcmd -E -S .\sqlexpress -i SKUD_Create.sql
sqlcmd -E -S .\sqlexpress -i InsertAllPatches.sql
sqlcmd -E -S .\sqlexpress -i SKUD_SP_Save.sql
sqlcmd -E -S .\sqlexpress -i SKUD_Data.sql
sqlcmd -E -S .\sqlexpress -i SP_TESTDATA.sql
del c:\passportDesu.jpg
del c:\image1.jpg
del c:\image2.jpg

pause