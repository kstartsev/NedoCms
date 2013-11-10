$(document).ready(function() {
	$().dndPageScroll();
});

function onDragStart(ev, settings) {
	ev.dataTransfer.setData("Text", settings);
	var frame = $('#ContentEditor')[0];
	if (frame && frame.contentDocument) {
		$(frame.contentDocument).find('.page-content-item-drop-area').addClass('colored');
	}
}
function onDragEnd() {
	var frame = $('#ContentEditor')[0];
	if (frame && frame.contentDocument) {
		$(frame.contentDocument).find('.page-content-item-drop-area').removeClass('colored');
	}
}

if (!NedoCms) var NedoCms = {};
if (!NedoCms.Editor) NedoCms.Editor = {};
NedoCms.Editor.postMessage = function(data) {
	var offset = 60;
	if (data && data.origin == 'sizechange') {
		var w = (data.width || 0);
		var h = (data.height || 0);
		$('#ContentEditor').css({
			height: (h + offset) + "px",
			width: w + "px",
		});
		$('#ContentHeader').css({ width: w + "px" });
	}
};
NedoCms.Editor.showEditor = function (url) {
	var editor = $('#editor-modal');
	$.get(url).success(function (html) {
		editor.find('.modal-content').html(html);
		editor.modal('show');

		var form = editor.find('form');
					
		var showloader = function (result) {
			var loader = form.find('.modal-footer .loading');
			var buttons = form.find('.modal-footer .btn');
			if (!loader || !loader.length) return;

			if (result) {
				loader.show();
				buttons.hide();
			} else {
				loader.hide();
				buttons.show();
			}
		};
		var remote = $.validator.methods.remote;
		$.validator.methods.remote = function (value, element, param) {
			param.complete = function (res) {
				var valid = res && res.responseText && res.responseText == "true";
				if (!valid) {
					showloader(false);
				}
			};
			return remote.apply(this, [value, element, param]);
		};
		$.validator.unobtrusive.parse(editor[0]);

		//attaching hooks for jQuery validation
		form.addTriggersToJqueryValidate().triggerElementValidationsOnFormValidation();
		form.formValidation(function (element, result) { showloader(result); });
		form.elementValidation(function (element, result) {
			var group = $(element).closest('.form-group');
			if (!result) group.addClass('has-error');
			else group.removeClass('has-error');
		});

		// applying content editors
		form.find('.content-editable').each(function () {
			var input = $(this);

			var id = NedoCms.Utils.guid();

			var element = $("<div></div>")
				.attr({ 'contenteditable': true, 'class': 'form-control', 'id': id })
				.bind('blur keyup paste copy cut mouseup', function () {
					input.val(element.html());
				})
				.html(input.val());

			input.closest('.form-group').append(element);

			CKEDITOR.inline(id);
		});
	}).error(function () { NedoCms.Editor.error('Error dialog', 'Cannot load page editor.'); });
};
NedoCms.Editor.error = function(title, content) {
	var mdl = $('#error-modal');
	mdl.find('.modal-title').html(title);
	mdl.find('.modal-body p').html(content);

	mdl.modal('show');
};
NedoCms.Editor.confirm = function(title, content, onconfirm) {
	var mdl = $('#confirmation-modal');
	mdl.find('.modal-title').html(title);
	mdl.find('.modal-body p').html(content);
	mdl.one('click', '.btn-success', function() {
		mdl.modal('hide');
		if (onconfirm) {
			onconfirm();
		}
	});
	mdl.modal('show');
};
NedoCms.Editor.renderModal = function(context) {
	context.instantiate(function(html) {
		var div = $('<div></div>')
			.attr({ id: NedoCms.Utils.guid() })
			.css({ display: 'none' })
			.append(html);

		$('#editors-placeholder').append(div);

		$.validator.unobtrusive.parse(div[0]);

		var modalDialog = div.find('.modal');
		modalDialog.find('button:submit').click(function() {
			var form = modalDialog.find('form');
			var validate = form.validate();
			if (!validate.form()) {
				return false;
			}
			form.iframePostForm({
				json: true,
				complete: function(r) {
					context.save(modalDialog, r);
					modalDialog.modal('hide');
				}
			});
			return true;
		});
		modalDialog.modal('show').on('hidden', function() { div.remove(); });
		div.show();
	});
};
NedoCms.Editor.saveContent = function(btn) {
	$(btn).button('loading');
	$('.modal').each(function() {
		$(this).modal('hide');
	});

	var frame = $('#ContentEditor')[0];
	if (frame && frame.contentWindow && frame.contentWindow.getEditorValues) {
		var values = frame.contentWindow.getEditorValues();

		$.ajax({
			url: window.$$SaveEditorContentUrl,
			type: "POST",
			data: JSON.stringify({ id: window.$$PageId, result: { items: values } }),
			contentType: "application/json; charset=utf-8",
			dataType: "json",
			cache: false,
			success: function() {
				$('#save-confirmation-modal').modal('show');
				$(btn).button('reset');
			},
			error: function() {
				NedoCms.Editor.error('Failed to save page.', 'Error happened while saving page content&hellip;');
				$(btn).button('reset');
			}
		});
	}
};
