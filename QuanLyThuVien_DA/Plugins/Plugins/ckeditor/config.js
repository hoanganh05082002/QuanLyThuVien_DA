﻿/*
Copyright (c) 2003-2012, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';

	//config.syntaxhighlight_lang = 'csharp';
	//config.syntaxhighlight_hideControls = true;
	config.language = 'vi';

	//CKFinder.setupCKEditor(null, '~/Assets/Plugins/ckfinder/');

	config.filebrowserBrowseUrl = '/Assets/Plugins/ckfinder/ckfinder.html';
	config.filebrowserUploadUrl = '/Assets/Plugins/ckfinder/core/connector/aspx/connector.aspx?command=QuickUpload&type=Files';
};
