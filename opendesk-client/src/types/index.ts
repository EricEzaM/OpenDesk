// API RESPONSES

export interface ApiResponse<T> {
	success: boolean;
	data?: T;
	problem?: ProblemDetails;
}

export interface ValidationError {
	message: string;
	errorCode: string;
	attemptedValue: unknown;
	state: unknown;
	severity: number;
}

export interface ProblemDetails {
	type: string;
	title: string;
	status: number;
	detail: string;
	instance: string;
	[extension: string]: number | string | ValidationError[];
}

// BOOKING

export interface Booking {
	id: string;
	deskId: string;
	startDateTime: Date;
	endDateTime: Date;
	user: User;
}

export interface FullBooking {
	id: string;
	startDateTime: Date;
	endDateTime: Date;
	office: Office;
	desk: Desk;
	user: User;
}

// DESK

export interface Desk {
	id: string;
	name: string;
	diagramPosition: DiagramPosition;
}

export interface DiagramPosition {
	x: number;
	y: number;
}

// OFFICE

export interface Office {
	id: string;
	location: string;
	subLocation: string;
	name: string;
	image: Blob;
}

// USER

export interface User {
	id: string;
	username: string;
	displayName: string;
}

export interface Role {
	id: string;
	name: string;
	description: string;
}

// BLOB

export interface Blob {
	id: string;
	uri: string;
	expiry: Date;
}
