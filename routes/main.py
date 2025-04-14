from datetime import datetime
from flask import Blueprint, jsonify, request, abort, Response
import json
import os
import requests

main = Blueprint('main', __name__)

@main.route("/", methods=["GET"])
def index():
    return Response(f"200 OK", mimetype='text/plain')

@main.route('/api/check', methods=['POST'])
def check_json():
    # Try to parse the JSON body
    data = request.get_json()    
    if not data or 'amount' not in data:
        abort(500, description="'amount' key missing in JSON body")
    try:
        amt = float(data.get('amount'))
    except (ValueError, TypeError):
        abort(500, description="Invalid")    
    if amt < 25000:
        abort(500, description="Invalid")
    else:
        return jsonify({"message": "Valid", "data": data}), 200
