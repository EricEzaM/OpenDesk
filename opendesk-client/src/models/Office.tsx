export default interface Office {
	id: string;
	name: string;
	image: OfficeImage;
}

export interface OfficeImage {
	url: string;
	width: number;
	height: number;
}
