import { set } from "date-fns";
import React from "react";
import { default as ReactDatePicker } from "react-datepicker";

interface DatePickerProps {
	selected?: Date;
	onChange?: (date: Date) => void;
	placeholderText?: string;
}

function DatePicker({ selected, onChange, placeholderText }: DatePickerProps) {
	function disallowPast(date: Date) {
		let currentDate = set(new Date(), {
			hours: 0,
			minutes: 0,
			seconds: 0,
			milliseconds: 0,
		});
		let dateOnly = set(date, {
			hours: 0,
			minutes: 0,
			seconds: 0,
			milliseconds: 0,
		});

		return dateOnly >= currentDate;
	}

	return (
		<ReactDatePicker
			placeholderText={placeholderText}
			selected={selected}
			onChange={(date) =>
				date && date instanceof Date && onChange && onChange(date)
			}
			dateFormat="MMMM d, yyyy h:mm aa"
			filterDate={disallowPast}
			showTimeInput
			customTimeInput={React.createElement(CustomTimeInput)}
		/>
	);
}

export default DatePicker;

interface CustomTimeInputProps {
	value: string;
	onChange: (event: string) => void;
}

function CustomTimeInput({ value, onChange }: CustomTimeInputProps) {
	return (
		<select value={value} onChange={(e) => onChange(e.target.value)}>
			<option value="06:00">6:00 AM</option>
			<option value="08:00">8:00 AM</option>
			<option value="10:00">10:00 AM</option>
			<option value="12:00">12:00 PM</option>
			<option value="14:00">2:00 PM</option>
			<option value="16:00">4:00 PM</option>
			<option value="18:00">6:00 PM</option>
			<option value="20:00">8:00 PM</option>
		</select>
	);
}
