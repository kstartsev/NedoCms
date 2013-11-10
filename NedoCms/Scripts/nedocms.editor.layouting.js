window.NedoCms = $.extend({}, window.NedoCms, window.parent.NedoCms);

if (!window.NedoCms.Layouting) {
	window.NedoCms.Layouting = function () {
		if (window.NedoCms._layouting) return window.NedoCms._layouting;

		window.NedoCms._layouting = {
			resize: function () {
				if (parent && parent.window) {
					var height = document.body.scrollHeight;
					var width = document.body.scrollWidth;
					NedoCms.Editor.postMessage({ origin: 'sizechange', height: height, width: width }, "*");
				}
			},
			ckeditor: {
				create: function (editorid) {
					try {
						var editor = $('#' + editorid);
						if (!editor.attr('contenteditable')) return;

						if (!editor.data('detached')) {
							if (!CKEDITOR.instances[editorid]) { CKEDITOR.inline(editorid); }

							editor
								.unbind('blur keyup paste copy cut mouseup')
								.bind('blur keyup paste copy cut mouseup', function () { NedoCms.Layouting().resize(); })
								.data('detached', false);
						}
					} catch (e) { }
				},
				destroy: function (editorid) {
					try {
						var instance = CKEDITOR.instances[editorid];
						if (instance) { instance.destroy(); }

						$('#' + editorid)
							.unbind('blur keyup paste copy cut mouseup')
							.data('detached', true);
					} catch (e) { }
				}
			},
			interactions: function (container) {
				var removeEditor = function () {
					NedoCms.Editor.confirm('Delete confirmation', 'Are you sure?', function () {
						container.remove();
					});
				};
				var dialog = container.find('.item-editor').data('edit-dialog');
				container.find('.item-editor-header .edit-button').hide();
				container
					.on('click', '.item-editor-header .remove-editor', function (e) {
						if (e && e.preventDefault) e.preventDefault();
						removeEditor();
						return false;
					})
					.on('click', '.item-editor-header .edit-button', function (e) {
						if (e && e.preventDefault) e.preventDefault();
						if (window[dialog]) {
							window[dialog](container.find('.item-editor'));
						}
						return false;
					});
				if (dialog) {
					container.find('.item-editor-header .edit-button').show();
				}
				$.contextMenu({
					selector: '#{0} .item-editor-header'.format(container.attr('id')),
					callback: function (key) { if (key != 'remove') { removeEditor(); } },
					items: { "remove": { name: "Remove action", icon: "remove"} }
				});
			},
			getEditorValue: function (editor) {
				var valueType = editor.data('value-type');
				if (valueType == 'html') return editor.html();
				if (valueType == 'script') return editor.find('textarea').val();
				if (valueType == 'none') return '';
				if (valueType == 'custom') {
					var getter = editor.data('value-getter');
					if (getter && window[getter]) {
						return window[getter](editor);
					}
				}
				return '';
			}
		};
		return window.NedoCms._layouting;
	};
}

$(document).ready(function () {
	// adding sorting for already added items
	$('.page-content-item').sortable({
		items: '.item-editor-container',
		connectWith: '.page-content-item',
		helper: 'original',
		tolerance: 'pointer',
		opacity: 0.8,
		handle: '.sort-handle'
	}).on("sortstart", function (event, ui) {
		try {
			var container = $(ui.item).find('.item-editor');
			if (container.data('value-type') == 'html') {
				var editor = CKEDITOR.instances[container.attr('id')];
				if (editor) {
					editor.destroy();
				}
			}
		} catch (e) { }
	}).on("sortstop", function (event, ui) {
		var container = $(ui.item).find('.item-editor');
		if (container.data('value-type') == 'html') {
			NedoCms.Layouting().ckeditor.create(container.attr('id'));
		}
	}).on("sortover", function () {
	}).on("sortout", function () {
	}).on("sortchange", function (event, ui) {
		var placeholder = $(ui.placeholder).closest('.page-content-item');
		var droparea = placeholder.find('.page-content-item-drop-area');
		var clone = droparea.clone();
		droparea.remove();
		placeholder.append(clone);
	});

	NedoCms.Layouting().resize();
});

// invokation is set in C# code
function onDragOver(ev) {
	ev.preventDefault();
	$(ev.target).addClass('hovered');
}
function onDragLeave(ev) {
	ev.preventDefault();
	$(ev.target).removeClass('hovered');
}

function onDrop(ev) {
	ev.preventDefault();
	
	$(ev.target).removeClass('hovered');

	// IE supports only 'Text' and 'Url' data keys
	var settings = ev.dataTransfer.getData("Text");
	$.post(window.$$EditorForPageContentUrl, { settings: settings })
		.always(function() { NedoCms.Layouting().resize(); })
		.success(function(html) { $(ev.target).before(html); });
}

// invoked from parent window
function getEditorValues() {
	var result = [];
	$('.page-content-item').each(function () {
		var container = $(this);
		container.find('.item-editor').each(function () {
			var editor = $(this);
			var parent = editor.parent();
			result.push({
				PageId: window.$$PageId,
				Id: parent.data('contentid'),
				Placeholder: container.data('placeholder-name'),
				Content: NedoCms.Layouting().getEditorValue(editor),
				Settings: parent.data('settings'),
				SharedId: parent.data('shared')
			});
		});
	});
	return result;
}