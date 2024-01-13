#! /bin/sh

set -e

if [ -z "$2" ]; then
    echo "Usage: run-server.sh {fleet-id} {auth-token}"
    exit 1
fi

WEBSOCKET_URL="wss://us-west-2.api.amazongamelift.com"
PROCESS_ID=`uuid`
HOST_ID="godot-demo"
FLEET_ID=$1
AUTH_TOKEN=$2

echo "Running GameLift Anywhere server $PROCESS_ID in fleet $FLEET_ID ..."

../Builds/"Godot Multiplayer Demo.sh" --headless --gamelift --webSocketUrl="$WEBSOCKET_URL" --processId="$PROCESS_ID" --hostId="$HOST_ID" --fleetId="$FLEET_ID" --authToken="$AUTH_TOKEN"
