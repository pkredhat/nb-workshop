
#!/bin/bash
# curl_random.sh
# Randomly send either a valid or an invalid POST request to /api/check

if [ "$#" -ne 1 ]; then
  echo "Usage: ./api_check.sh host:port (i.e. localhost:5000)" >&2
  exit 1
fi

# Generate a random number: either 0 or 1.
if [ $(( RANDOM % 2 )) -eq 0 ]; then
  echo "$(date): Sending valid request"
  # Valid request: includes the required "musthave" key in JSON.
  curl -s -X POST -H "Content-Type: application/json" \
       -d '{"musthave": "somevalue", "other": "optional"}' \
       http://$1/api/check -v
  echo ""  # Optional: adds a newline for clarity.
else
  echo "$(date): Sending invalid request"
  # Invalid request: missing the "musthave" key.
  curl -s -X POST -H "Content-Type: application/json" \
       -d '{"notmusthave": "value"}' \
       http://$1/api/check -v
  echo ""
fi
