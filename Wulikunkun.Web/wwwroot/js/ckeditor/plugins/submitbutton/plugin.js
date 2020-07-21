CKEDITOR.plugins.add('submitbutton', {
    icons: 'submitbutton',
    init: function (editor) {
        $("input[type='submit']").hide();
        editor.addCommand('insertTimestamp', {
            exec: function (editor) {
                $("input[type='submit']").click();
            }
        });

        editor.ui.addButton('submitbutton', {
            label: '点击我进行提交',
            command: 'insertTimestamp',
            toolbar: 'editing',
            icon: this.path + 'icons/submitbutton.png'  // icon file (PNG)
        });
    }
});