#!/bin/bash

/opt/mssql/bin/sqlservr &

echo "waiting SQL Server..."
for i in {1..30}; do
    sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" &> /dev/null
    if [ $? -eq 0 ]; then
        echo "SQL Server ready"
        break
    fi
    echo "waiting SQL Server..."
    sleep 1
done

echo "DB Medilabo creation ..."
sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "CREATE DATABASE Medilabo"

wait