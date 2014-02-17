sqlcmd -E -S .\sqlexpress -i SKUD_Create.sql
sqlcmd -E -S .\sqlexpress -i SKUD_Relations.sql
sqlcmd -E -S .\sqlexpress -i SKUD_SP_Save.sql
sqlcmd -E -S .\sqlexpress -i SP_TESTDATA.sql