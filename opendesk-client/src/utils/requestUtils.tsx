interface JsonParseResult {
	ok: boolean;
	json: any;
}

function parseJson(response: Response): Promise<JsonParseResult> {
	return new Promise((resolve) =>
		response.json().then((json) =>
			resolve({
				ok: response.ok,
				json: json,
			})
		)
	);
}

export default function apiRequest(
	url: string,
	options?: RequestInit
): Promise<any> {
	return new Promise((resolve, reject) => {
		fetch(process.env.REACT_APP_API_URL + "/api" + url, options)
			.then(parseJson)
			.then((response) => {
				if (response.ok) {
					return resolve(response.json);
				}

				return reject(response.json.error);
			})
			.catch((error) => reject(error.message));
	});
}
