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
	name: string;
	image: OfficeImage;
}

export interface OfficeImage {
	url: string;
	width: number;
	height: number;
}

// USER

export interface User {
	id: string;
	name: string;
	username: string;
}
