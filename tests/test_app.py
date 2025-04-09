import pytest
import json
from app import app
from unittest.mock import patch

@pytest.fixture
def client():
    app.config['TESTING'] = True
    with app.test_client() as client:
        yield client

def test_missing_translation_key(client, monkeypatch):
    # This test assumes the COUNTRY_CODE env var is not used in the current route implementation,
    # but will still check for the error case when a translation is missing.
    monkeypatch.setenv("COUNTRY_CODE", "xx")  # Assume "xx" is not in the translations file.
    response = client.get("/")
    # The index route calls abort(500) if translation fails, so we expect a 500 error.
    assert response.status_code == 500
    assert isinstance(response.data.decode(), str)

def test_index_success(client):
    with patch("routes.main.get_translation") as mock_get_translation, \
         patch("routes.main.get_current_datetime") as mock_get_current_datetime:
        
        # Use a value that will be lowercased in the index route.
        mock_get_translation.return_value = "Hello"
        # Use the NYC datetime format: "HH:MM MM/DD/YYYY"
        mock_get_current_datetime.return_value = "12:00 04/03/2025"

        response = client.get("/")
        assert response.status_code == 200
        # The index endpoint transforms the translation to lower case.
        expected_response = "hello @ 12:00 04/03/2025"
        assert response.data.decode("utf-8") == expected_response
        assert response.mimetype == "text/plain"

def test_index_failure(client):
    with patch("routes.main.get_translation", side_effect=Exception("Something went wrong")):
        response = client.get("/")
        assert response.status_code == 500
        assert b"Something went wrong" in response.data

def test_get_translation_direct(tmp_path, monkeypatch):
    translations_data = {
        "translations": {
            "EN": "hello world",
            "FR": "bonjour monde!"
        }
    }
    # Write the translations JSON file to the temporary directory.
    path = tmp_path / "translations.json"
    path.write_text(json.dumps(translations_data), encoding="utf-8")
    # Change the working directory so that get_translation can find translations.json.
    monkeypatch.chdir(tmp_path)

    # Import the function after changing the directory.
    from routes.main import get_translation

    # Valid cases.
    assert get_translation("EN") == "hello world"
    assert get_translation("FR") == "bonjour monde!"

    # Invalid key should raise an Exception.
    with pytest.raises(Exception, match="Translation not found"):
        get_translation("JP")
