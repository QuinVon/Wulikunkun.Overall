; (function () {
    var factory = function (exports) {
        var $ = jQuery; // if using module loader(Require.js/Sea.js).
        exports.postbutton = function () { };
        exports.fn.postbutton = function () {
            $("i.fa.fa-paper-plane").click(function () {
                $("#SubmitModal").modal("show");
                $("textarea[name='htmlcontent']").val(editor.getPreviewedHTML());
            });
        };
    };

    // CommonJS/Node.js
    if (
        typeof require === "function" &&
        typeof exports === "object" &&
        typeof module === "object"
    ) {
        module.exports = factory;
    } else if (typeof define === "function") {
        // AMD/CMD/Sea.js
        if (define.amd) {
            // for Require.js
            define(["editormd"], function (editormd) {
                factory(editormd);
            });
        } else {
            // for Sea.js
            define(function (require) {
                var editormd = require("../../editormd");
                factory(editormd);
            });
        }
    } else {
        factory(window.editormd);
    }
})();
