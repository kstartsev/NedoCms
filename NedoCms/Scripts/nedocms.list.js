(function($) {
	$.fn.listEx = function(options) {
		var defaults = {
			OrderBy: null, // specifies default sorting field
			OrderByDirection: null, // specifies default sorting direction
			pager: null, // specfies pager container
			totalPages: 1, // specifies total number of the pages,
			currentPage: 0, // sepcifies currently selected page
			onOrderClick: function(orderBy, direction) { // specifies handler for order clickc event
				var q = new Query(document.location.search);
				q.set('OrderBy', orderBy);
				q.set('OrderByDirection', direction);
				q.set('CurrentPageIndex', null);
				document.location.search = q.toString();
			},
			onPageClick: function(index) { // specifies default handler for page click
				var q = new Query(document.location.search);
				q.set('CurrentPageIndex', index);
				document.location.search = q.toString();
			},
			pageRenderer: function(index, current, total) { // specifies page element renderer
				var intervalLength = 18;
				//rendering first/prev
				if (current >= intervalLength) {
					var prev = (Math.floor(current / intervalLength) * intervalLength) - 1;
					if (index <= prev) {
						if (index == 1) {
							return $('<a href="javascript:void(0)" class="first" data-page="' + index + '">First</a>');
						}
						if (index == prev) {
							return $('<a href="javascript:void(0)" class="prev" data-page="' + index + '"></a>').html('Previous ' + intervalLength);
						}
						return null;
					}
				}
				//rendering next/last
				var t = Math.floor(total % intervalLength);
				if (current < (total - t)) {
					var next = ((Math.floor(current / intervalLength) + 1) * intervalLength);
					if (index >= next) {
						if (index == next) {
							return $('<a href="javascript:void(0)" class="next" data-page="' + index + '"></a>').html('Next ' + intervalLength);
						}
						if (index == total) {
							return $('<a href="javascript:void(0)" class="last" data-page="' + index + '">Last</a>');
						}
						return null;
					}
				}
				if (index == current) {
					return $('<span class="active"></span>').html(i);
				}
				return $('<a href="javascript:void(0)" data-page="' + index + '"></a>').html(index);
			},
			orderRenderer: function(elem, dir) { // specifies renderer for order sign
				elem.removeClass("down up").addClass(dir == "desc" ? "down" : "up");
			}
		};
		var settings = $.extend({}, defaults, options);

		var element = this;

		var query = new Query(document.location.search);
		query.set('OrderByDirection', (query.get('OrderByDirection') || settings.OrderByDirection) || 'asc');
		query.set('OrderBy', query.get('OrderBy') || settings.OrderBy);

		if (!query.get('OrderBy')) {
			var name = element.find("a[id^='orderby_']").first().attr("id");
			if (name) {
				query.set('OrderBy', name.replace(/orderby_/gi, ""));
			}
		}
		element.find("a[id^='orderby_']").click(function(e) {
			if (e && e.preventDefault) e.preventDefault();

			var orderBy = this.id.replace(/orderby_/gi, "");
			var direction = (orderBy != query.get('OrderBy', true) || query.get('OrderByDirection', true) == "desc") ? "asc" : "desc";

			if (settings.onOrderClick) {
				settings.onOrderClick(orderBy, direction);
			}

			return false;
		});
		if (settings.orderRenderer) {
			var elementname = ('orderby_' + query.get('OrderBy')).toString().toLowerCase();
			settings.orderRenderer(element.find("[id='" + elementname + "']"), query.get('OrderByDirection'));
		}

		//adding pagination
		if (settings.pager && settings.pageRenderer) {
			var pageClick = function(index, e) {
				if (e && e.preventDefault) e.preventDefault();

				if (settings.onPageClick) {
					settings.onPageClick(index - 1);
				}
				return false;
			};
			var count = parseInt(settings.totalPages || 1);
			//showing pagination only if we have more than 1 page
			if (count > 1) {
				settings.pager.show();
				var cur = parseInt(settings.currentPage || 0);
				for (var i = 1; i <= count; i++) {
					var rendered = settings.pageRenderer(i, cur, count);
					if (!rendered) continue;

					(function(index, layout) {
						if (layout.is('a')) {
							layout.click(function(e) {
								var page = index;

								var a = $(this);
								if (a.attr('data-page')) {
									page = parseInt(a.attr('data-page'));
								}

								return pageClick(page, e);
							});
						} else {
							layout.find('a').click(function(e) { return pageClick(index, e); });
						}

						settings.pager.append(layout);
					})(i, rendered);
				}
			}
		}

		return this;
	};
})(jQuery);