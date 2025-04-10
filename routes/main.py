from datetime import datetime
from flask import Blueprint, jsonify, request, abort, Response
import json
import os
import requests

main = Blueprint('main', __name__)



@main.route("/", methods=["GET"])
def index():
    return Response(f"Boiler plate home.", mimetype='text/plain')


@main.route("/api/force500")
def force_500():
    abort(500, description="Intentional 500 error for demonstration purposes")

@main.route("/api/force200")
def force_200():
    return "This is a forced 200 OK response.", 200

@main.route('/api/check', methods=['POST'])
def check_json():
    # Try to parse the JSON body
    data = request.get_json()

    # Check if JSON was provided and if "musthave" is in the JSON object.
    if not data or 'musthave' not in data:
        # Aborting with a 500 error if "musthave" is missing
        abort(500, description="'musthave' key missing in JSON body")

    # Return a 200 OK response with a simple JSON message if key is present.
    return jsonify({"message": "'musthave' key received", "data": data}), 200