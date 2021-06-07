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

		try {
			// Valid JSON
			let json = await response.json();
			const isProblem = response.headers
				.get("content-type")
				?.includes("application/problem+json");

			resolve({
				status: response.status,
				data: isProblem ? undefined : json,
				problem: isProblem ? json : undefined,
			});
		} catch (error) {
			// Invalid JSON
			resolve({
				status: response.status,
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
