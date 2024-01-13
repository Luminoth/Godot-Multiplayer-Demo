#! /bin/sh

set -e

AWS=aws

# location name regex - custom-[A-Za-z0-9\-]
LOCATION_NAME="custom-godot-demo-location-1"
FLEET_NAME="godot-demo-fleet-1"
HARDWARE_ID="godot-demo"
IP=`hostname --ip-address`

echo "Creating GameLift resources..."

echo "Creating Location $LOCATION_NAME ..."
$AWS gamelift create-location --location-name $LOCATION_NAME

echo "Creating Fleet $FLEET_NAME in $LOCATION_NAME ..."
FLEET_ID=`$AWS gamelift create-fleet --name $FLEET_NAME --compute-type ANYWHERE --locations "Location=$LOCATION_NAME" --query 'FleetAttributes.FleetId' | tr -d '"'`

echo "Fleet ID: $FLEET_ID"

echo "Registering Compute $HARDWARE_ID at $IP in $FLEET_ID / $LOCATION_NAME..."
$AWS gamelift register-compute --compute-name $HARDWARE_ID --fleet-id $FLEET_ID --ip-address $IP --location $LOCATION_NAME

echo "Getting Compute Auth Token for $HARDWARE_ID in $FLEET_ID ..."
AUTH_TOKEN=`$AWS gamelift get-compute-auth-token --fleet-id $FLEET_ID --compute-name $HARDWARE_ID --query 'AuthToken' | tr -d '"'`

echo "Auth Token: $AUTH_TOKEN"

echo "Done!"
