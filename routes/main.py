from datetime import datetime
from flask import Blueprint, jsonify, request, abort, Response
from opentelemetry import trace
from opentelemetry.instrumentation.flask import FlaskInstrumentor
from opentelemetry.sdk.trace import TracerProvider
from opentelemetry.sdk.trace.export import BatchSpanProcessor
from opentelemetry.exporter.jaeger.thrift import JaegerExporter
from flask import Flask

# OpenTelemetry setup
trace.set_tracer_provider(TracerProvider())
tracer = trace.get_tracer(__name__)

jaeger_exporter = JaegerExporter(
    agent_host_name=os.getenv("JAEGER_HOST", "https://tempo-sample-query-frontend-observability.apps.nb-demo-01.6jz4.p1.openshiftapps.com/"),
    agent_port=int(os.getenv("JAEGER_PORT", "6831")),
)

span_processor = BatchSpanProcessor(jaeger_exporter)
trace.get_tracer_provider().add_span_processor(span_processor)

# Flask app + instrumentation
app = Flask(__name__)
FlaskInstrumentor().instrument_app(app)

main = Blueprint('main', __name__)

@main.route("/", methods=["GET"])
def index():
    return Response(f"200 OK", mimetype='text/plain')


@main.route('/api/check', methods=['POST'])
def check_json():
    span = trace.get_current_span()

    data = request.get_json()
    if not data or 'amount' not in data:
        span.set_attribute("error", True)
        span.set_attribute("http.status_code", 500)
        span.set_attribute("error.message", "'amount' key missing in JSON body")
        abort(500, description="'amount' key missing in JSON body")

    try:
        amt = float(data.get('amount'))
    except (ValueError, TypeError):
        span.set_attribute("error", True)
        span.set_attribute("http.status_code", 500)
        span.set_attribute("error.message", "Invalid amount format")
        abort(500, description="Invalid")

    if amt < 25000:
        span.set_attribute("error", True)
        span.set_attribute("http.status_code", 500)
        span.set_attribute("error.message", "Amount below minimum threshold")
        abort(500, description="Invalid")

    return jsonify({"message": "Valid", "data": data}), 200


# Register blueprint and start app if needed
app.register_blueprint(main)

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=8080)