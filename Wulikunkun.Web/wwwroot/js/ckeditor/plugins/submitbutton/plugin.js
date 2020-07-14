﻿CKEDITOR.plugins.add('submitbutton', {
    icons: 'submitbutton',
    init: function (editor) {
        editor.addCommand('insertTimestamp', {
            exec: function (editor) {
                var now = new Date();
                editor.insertHtml('The current date and time is: <em>' + now.toString() + '</em>');
            }
        });
        editor.ui.addButton('SubmitButton', {
            label: 'Insert Timestamp',
            command: 'insertTimestamp',
            toolbar: 'editing',
            icon: this.path + '/icons/submitbutton.png'  // icon file (PNG)
        });
    }
});