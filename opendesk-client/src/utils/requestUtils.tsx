interface ApiCallResult {
	ok: boolean;
	code: number;
	data: any;
}

function parseJson(response: Response): Promise<ApiCallResult> {
	return new Promise((resolve) =>
		response.text().then((text) => {
			try {
				const data = JSON.parse(text);
				// Valid JSON
				return resolve({
					ok: response.ok,
					code: response.status,
					data: data,
				});
			} catch {
				// Not Valid JSON
				resolve({
					ok: response.ok,
					code: response.status,
					data: {},
				});
			}
		})
	);
}

export default function apiRequest(
	url: string,
	options?: RequestInit
): Promise<ApiCallResult> {
	return new Promise((resolve, reject) => {
		options = {...options, credentials: "include" }
		fetch(process.env.REACT_APP_API_URL + "/api/" + url, options)
			.then(parseJson)
			.then((response) => resolve(response))
			.catch((error) => {
				console.error("An error occurred in the API fetch request.");
				console.error(error);
				reject(error.message);
			});
	});
}
