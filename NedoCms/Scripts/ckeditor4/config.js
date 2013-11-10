/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function(config) {
	// Define changes to default configuration here.
	// For the complete reference:
	// http://docs.ckeditor.com/#!/api/CKEDITOR.config

	config.enterMode = CKEDITOR.ENTER_BR;
	config.autoParagraph = false;
	config.allowedContent = true;
	config.skin = 'moono-color';
	config.toolbarCanCollapse = false;
	config.colorButton_enableMore = false;
	config.jqueryOverrideVal = true;
	config.resize_enabled = false;
	config.extraPlugins = "htmlSource";
	config.removePlugins = 'elementspath,sourcearea,contextmenu,liststyle,tabletools';

	if (window.$$initializeEditor) {
		window.$$initializeEditor(config);
	}
	config.toolbar = [
		['htmlSource'],
		['Format', 'Font', 'FontSize'],
		['Bold', 'Italic', 'Underline', '-', 'RemoveFormat'],
		['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord'],
		['TextColor', 'BGColor'],
		['Link', 'Unlink'],
		['Image', 'Table', 'HorizontalRule'],
		['NumberedList', 'BulletedList']
	];

	// Remove some buttons, provided by the standard plugins, which we don't
	// need to have in the Standard(s) toolbar.
	config.removeButtons = 'Underline,Subscript,Superscript';

	// Se the most common block elements.
	config.format_tags = 'p;h1;h2;h3;pre';

	// Make dialogs simpler.
	config.removeDialogTabs = 'image:advanced;link:advanced';
};
