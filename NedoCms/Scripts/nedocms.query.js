/**
* Represents a query string.
*/
Query = function (queryString) {
    this.append(queryString);
};

Query.prototype =
{
	append: function (queryString) {
		if (!queryString) {
			queryString = "";
		} else if (queryString[0] == "?") {
			queryString = queryString.substring(1);
		}

		var regex = /([^&=]+)=([^&]*)/g;

		var match;
		while (match = regex.exec(queryString)) {
			this[decodeURIComponent(match[1])] = decodeURIComponent(match[2]);
		}
	},

	appendForm: function (form) {
		var formValues = form.serializeArray();
		var count = formValues.length;
		for (var index = 0; index < count; index++) {
			var pair = formValues[index];
			var name = pair.name;
			var value = pair.value || "";
			this[name] = value;
		}
	},
	/**
	* Filters the displayed series.
	*/
	toString: function () {
		var builder = new Array();
		for (var property in this) {
			if (typeof (this[property]) == "function") {
				continue;
			}
			var val = this[property];
			if (val == null || val == undefined || val === '') {
				continue;
			}
			builder.push(encodeURIComponent(property) + "=" + encodeURIComponent(val));
		}
		return "?" + builder.join("&");
	},
	// case insensitive property setter
	set: function (property, value) {
		if (!property) return false;

		this[property] = value;

		return true;
	},
	get: function (property, lower) {
		if (!property) return null;

		var p = property.toLowerCase();
		if (this[p]) {
			return lower ? this[p].toString().toLowerCase() : this[p];
		}

		var value = this[property];
		if (value) return lower ? value.toString().toLowerCase() : value;

		return null;
	}
};
