from __future__ import annotations

import base64
import json
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from urllib import error, parse, request


API_BASE_URL = "https://api.track.toggl.com"


class TogglApiError(RuntimeError):
    pass


class TogglFeatureUnavailableError(TogglApiError):
    pass


@dataclass(frozen=True)
class TogglResponse:
    status_code: int
    headers: dict[str, str]
    body: bytes


class TogglClient:
    def __init__(self, api_token: str, created_with: str) -> None:
        if not api_token or api_token == "REPLACE_WITH_TOGGL_API_TOKEN":
            raise ValueError("config.local.json must contain a real toggl_api_token before network commands can run.")
        self._api_token = api_token
        self._created_with = created_with

    def fetch_time_entries(self, start_at: datetime, end_at: datetime) -> list[dict]:
        start_value = _isoformat_utc(start_at)
        end_value = _isoformat_utc(end_at)
        query = parse.urlencode({"start_date": start_value, "end_date": end_value})
        response = self._request("GET", f"/api/v9/me/time_entries?{query}")
        return json.loads(response.body.decode("utf-8"))

    def create_time_entry(self, workspace_id: int, payload: dict) -> dict:
        response = self._request(
            "POST",
            f"/api/v9/workspaces/{workspace_id}/time_entries",
            payload=payload,
        )
        return json.loads(response.body.decode("utf-8"))

    def export_detailed_pdf(self, workspace_id: int, payload: dict, output_path: Path) -> Path:
        response = self._request(
            "POST",
            f"/reports/api/v3/workspace/{workspace_id}/search/time_entries.pdf",
            payload=payload,
            accept="application/pdf",
        )
        output_path.parent.mkdir(parents=True, exist_ok=True)
        output_path.write_bytes(response.body)
        return output_path

    def _request(self, method: str, path: str, payload: dict | None = None, accept: str = "application/json") -> TogglResponse:
        url = f"{API_BASE_URL}{path}"
        body: bytes | None = None
        headers = {
            "Accept": accept,
            "Authorization": _basic_auth_header(self._api_token),
            "User-Agent": self._created_with,
        }
        if payload is not None:
            body = json.dumps(payload).encode("utf-8")
            headers["Content-Type"] = "application/json; charset=utf-8"

        http_request = request.Request(url=url, data=body, headers=headers, method=method)
        try:
            with request.urlopen(http_request) as response:
                return TogglResponse(
                    status_code=response.getcode(),
                    headers=dict(response.headers),
                    body=response.read(),
                )
        except error.HTTPError as http_error:
            error_body = http_error.read()
            message = error_body.decode("utf-8", errors="replace")
            if http_error.code == 402:
                raise TogglFeatureUnavailableError(message) from http_error
            raise TogglApiError(f"HTTP {http_error.code}: {message}") from http_error


def _basic_auth_header(api_token: str) -> str:
    credentials = f"{api_token}:api_token".encode("utf-8")
    return f"Basic {base64.b64encode(credentials).decode('ascii')}"


def _isoformat_utc(value: datetime) -> str:
    utc_value = value.astimezone(timezone.utc).replace(microsecond=0)
    return utc_value.isoformat().replace("+00:00", "Z")
