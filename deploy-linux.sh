#!/bin/bash

PROJECT_HOME=/opt/incloud

echo "Making project folder /opt/incloud"
mkdir -p $PROJECT_HOME

# Preparing volumes

echo "Making volumes folder for MS SQL Server"
mkdir -p $PROJECT_HOME/db/mssqlserver/{data,log,secrets}
chmod -v -R 666 $PROJECT_HOME/db/mssqlserver/*
# We should provide the permissions on read/write for all users 
# to avoid the error like 'System cannot find the specified file' from MS SQL Server

echo "Making volumes folder for Redis"
mkdir -p $PROJECT_HOME/db/redis/data

# Init the deployment proccess
echo "Ok, let's build and run services..."
docker-compose up -d