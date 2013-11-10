String.prototype.format = function() {
	var args = arguments;
	return this.replace(/{(\d+)}/g, function(match, number) {
		return typeof args[number] != 'undefined'
			? args[number]
			: match;
	});
};

if (!NedoCms) var NedoCms = {};
if (!NedoCms.Utils) NedoCms.Utils = {};

NedoCms.Utils.guid = function() {
	return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
		var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
		return v.toString(16);
	});
};
NedoCms.Utils.updateCollection = function (rows, name, item) {
	if (item) {
		item.find('select, textarea, input:text').val('');
	}
	$.each(rows, function (i, r) {
		var row = $(r);

		row.find('a.delete').hide();
		if (rows.length > 1) row.find('a.delete').show();

		var index = Number(row.find("input[name='" + name + ".index']").val());

		row.find("input[name='" + name + ".index']").val(i);
		row.find('[id^=' + name + ']').each(function () { $(this).attr({ 'id': $(this).attr('id').replace(index, i), 'name': $(this).attr('name').replace(index, i) }); });
		row.find('[for^=' + name + ']').each(function () { $(this).attr('for', $(this).attr('for').replace(index, i)); });
		row.find('span[data-valmsg-for^=' + name + ']').each(function () { $(this).attr('data-valmsg-for', $(this).attr('data-valmsg-for').replace(index, i)); });
	});
};
NedoCms.Utils.query = function(key, value) {
	var query = new Query(window.location.search);
	query.set(key, value);
	return query.toString();
};
NedoCms.Utils.nopage = function(key, value) {
	var query = new Query(window.location.search);
	query.set('CurrentPageIndex', ''); // removing this from query
	query.set(key, value);
	return query.toString();
};
NedoCms.Utils.navigateTo = function (form) {
	var q = new Query(window.location.search);
	$.each($(form).serializeArray(), function (i, item) {
		q.set(item.name, item.value);
	});

	window.location.href = form.action + q.toString();
};
NedoCms.Utils.navigateToClean = function (form) {
	var q = new Query();
	$.each($(form).serializeArray(), function (i, item) {
		q.set(item.name, item.value);
	});

	window.location.href = form.action + q.toString();
};