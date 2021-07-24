import { ApiResponse } from "types";

export default async function apiRequest<T>(
	url: string,
	options?: RequestInit
): Promise<ApiResponse<T>> {
	return new Promise(async (resolve, reject) => {
		options = { ...options, credentials: "include" };

		let response = await fetch(
			process.env.REACT_APP_API_URL + "/api/" + url,
			options
		);

		const type = response.headers.get("content-type");

		const isProblem = type?.includes("application/problem+json");
		const isJson = type?.includes("application/json");

		try {
			if (isJson || isProblem) {
				// Valid JSON
				let json = await response.json();

				resolve({
					success: response.ok,
					data: isProblem ? undefined : json,
					problem: isProblem ? json : undefined,
				});
			} else {
				// Empty/unknown response - don't try parsing.
				resolve({
					success: response.ok,
					data: undefined,
					problem: undefined,
				});
			}
		} catch (error) {
			// Invalid JSON
			resolve({
				success: response.ok,
				problem: {
					status: response.status,
					title: "An invalid response was received from the API.",
					detail: "An invalid response was received from the API.",
					instance: "",
					type: "https://example.net/invalid-response", // todo: fix url
				},
			});
		}
	});
}
