sqlcmd -E -S .\FIRESECINSTANCE -i DropIfExists.sql
sqlcmd -E -S .\FIRESECINSTANCE -i Create.sql

pause