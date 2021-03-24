import { ApiResponse } from "types";

export default async function apiRequest<T>(
	url: string,
	options?: RequestInit
): Promise<ApiResponse<T> & { status: number }> {
	return new Promise(async (resolve, reject) => {
		options = { ...options, credentials: "include" };

		let response = await fetch(
			process.env.REACT_APP_API_URL + "/api/" + url,
			options
		);

		try {
			// Valid JSON
			let json = await response.json();
			resolve({
				status: response.status,
				...json,
			});
		} catch (error) {
			// Invalid JSON
			resolve({
				status: response.status,
				outcome: {
					isError: true,
					isValidationFailure: false,
					isSuccess: false,
					message: "An invalid response was received from the API.",
					errors: [],
				},
			});
		}
	});
}
