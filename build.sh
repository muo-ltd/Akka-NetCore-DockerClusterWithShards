#!/bin/bash
set -euo pipefail
IFS=$'\n\t'

function buildOne() {
    local DIR=$1
    
    pushd $DIR
    
    echoheading "Build: $DIR"
    dotnet restore
    dotnet build
    
    popd 
}

function buildAndPublishOne() {
    local DIR=$1
    
    pushd $DIR
    
    echoheading "Build and Publish: $DIR"
    dotnet restore
    dotnet build
    dotnet publish -c Release
    
    popd 
}

function package() {
    pushd ./src
    docker-compose build
    popd 
}

function run() {
    pushd ./src
    docker-compose up -d
    popd
}

function echoheading() {
    echo '*********************************************'
    echo '** '
    echo "** $1"
    echo '** '
    echo '*********************************************'
}

buildAndPublishOne ./src/client
buildAndPublishOne ./src/server
package
$@