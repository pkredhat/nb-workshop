from datetime import datetime
from flask import Blueprint, jsonify, request, abort, Response
import json
import os
import requests

main = Blueprint('main', __name__)

def get_translation(country_code):
    try:
        with open("translations.json", "r", encoding="utf-8") as f:
            data = json.load(f)
        translations = data.get("translations", {})
        translation = translations.get(country_code.upper())
        if not translation:
            raise Exception("Translation not found for the specified country code.")
        return translation
    except Exception as e:
        raise e

def get_current_datetime():
    return datetime.utcnow().isoformat() + "Z"



@main.route("/api/health", methods=["GET"])
def health():
    try:
        return "OK", 200
    except Exception:
        return "Service Unhealthy", 500



@main.route("/api/admin")
def admin_panel():    
    if request.args.get("password") == "opensesame":        
        response = requests.get(os.getenv("ADMIN"))
        if response.ok:
            data = json.loads(response.text)
            pretty_json = json.dumps(data, indent=4)
            
            # Wrap in <pre> so the browser displays newlines and spacing
            return f"<pre>{pretty_json}</pre>"
        else:
            abort(500, description="There was a problem calling the API, please review your parameters")
    return abort(403, description="Forbidden")



@main.route("/api/version", methods=["GET"])
def version():
    return "0.0.1", 200



@main.route("/", methods=["GET"])
def index():

    # STARTING COUNTRY CODE
    country_code = "en"

    try:
        translation = get_translation(country_code).lower()
        current_time = get_current_datetime()
    except Exception as e:
        abort(500, description=str(e))
    return Response(f"{translation} @ {current_time}", mimetype='text/plain')




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