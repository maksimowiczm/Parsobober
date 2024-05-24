#!/bin/sh

if [ -z "$1" ]; then
    echo "Usage: ./run.sh <test_case>"
    exit 1
fi

docker build -t spa-official .
docker run -v $1:/app/code -it --rm spa-official