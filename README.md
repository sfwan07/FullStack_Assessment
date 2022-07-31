# FullStack_Assessment

# By Safwan Osama

## Require to run

Docker => Linux Environment

## Before run

Change API URL to ip docker host in src\ui\src\environments\environment.prod.ts
Check your network

## To Run

1- open terminal to root project
2- write "docker-compose up -d" in terminal

### Notes

if docker-compose not support docker-compose or write to docker compose up or install this plugin

You can change port ui open compose.yml and look ui services name
Default port
UI => http://ip_docker_host:86
Consul => http://ip_docker_host:8500
API Gateway=> http://ip_docker_host:85

if you want run ui out from docker open terminal and write:

### require node.js in your host

1- open terminal to root project
2- write "cd src/ui" enter
3- write "npm i" enter
4- write "npm run start"
