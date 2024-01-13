#! /bin/sh

set -e

AWS=aws

if [ -z "$1" ]; then
    echo "Usage: cleanup-gamelift.sh {fleet-id}"
    exit 1
fi

# location name regex - custom-[A-Za-z0-9\-]
LOCATION_NAME="custom-godot-demo-location-1"
FLEET_ID=$1
HARDWARE_ID="godot-demo"

echo "Cleaning up GameLift resources..."

echo "Deregistering Compute $HARDWARE_ID in $FLEET_ID ..."
$AWS gamelift deregister-compute --compute-name $HARDWARE_ID --fleet-id $FLEET_ID

echo "Deleting Fleet $FLEET_ID ..."
$AWS gamelift delete-fleet --fleet-id $FLEET_ID

echo "Deleting Location $LOCATION_NAME ..."
$AWS gamelift delete-location --location-name $LOCATION_NAME

echo "Done!"
