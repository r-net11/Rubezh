sqlcmd -E -S .\sqlexpress -i DropIfExists.sql
sqlcmd -E -S .\sqlexpress -i Create.sql

pause