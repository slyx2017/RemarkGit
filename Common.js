//格式化日期格式为：yyyy-MM-dd
function FormatDate(now) {
	var year = now.getFullYear();
	var month = now.getMonth() + 1;
	var date = now.getDate();
	if (month < 10) {
		month = "0" + month.toString();
	}
	if (date < 10)
		date = "0" + date.toString();
	return year + "-" + month + "-" + date;
}
