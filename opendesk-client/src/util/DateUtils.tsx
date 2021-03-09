export function Get24hTime(date: Date) {
	return date.toLocaleTimeString([], {
		hour: "2-digit",
		minute: "2-digit",
	});
}

export function GetMonthDayFormatted(date: Date) {
	return date.toLocaleDateString([], {
		month: "numeric",
		day: "numeric",
	});
}

export function GetFullDateWith24hTime(date: Date) {
  date.toLocaleDateString([], {
    year: "numeric",
    month: "numeric",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  })
}