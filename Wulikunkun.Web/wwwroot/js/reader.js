; (function (jQuery, window, document, undefined) {
    function Reader(ele, options) {
        this.$ele = $(ele);

        this.defaults = {
            data: "",
            coverUrl: "",
        };

        this.settings = $.extend({}, this.defaults, options);

        /* 考虑到用户编写的内容中可能并没有父级标签包裹标题标签，所以在此手动使用div进行强制包裹 */
        this.$domData = $("<div>" + this.settings.data + "</div>");

        this.components = {
            $container: $(
                "<div class='container-fluid'><div class='row p-1' id='container'></div></div>"
            ),
            $leftPanel: $(
                '<div class="col-3 vh-100 overflow-auto position-relative px-2 left-panel custom-scroll custom-font"></div>'
            ),
            $leftPanelTopBar: $(
                '<div class="w-100 px-2 text-muted py-3 border-bottom border-light"><a href="/" class="text-light" title="返回首页"><i class="fa fa-home text-black-50" aria-hidden="true"></a></i><a class="float-right text-black-50 small" href="#">返 回</a></div>'
            ),
            $leftPanelCover: $(
                '<div class="mt-5"><img src="' +
                this.settings.coverUrl +
                '" class="d-block p-2 mx-auto my-2 custom-large-radius custom-w-40"/></div>'
            ),
            $leftPanelNavContainer: $('<nav class="nav-bar px-3 mt-5"></nav>'),
            /* 需要注意的就是这里的‘markdown-body editormd-preview-container’这两个class类是editor.md用来渲染背景颜色时需要用到的类，以后在改造插件的时候需要注意 */
            $rightPanel: $(
                '<div class="col-9 custom-light-panel-bg vh-100 overflow-auto position-relative px-5 rounded-lg"><div class="min-vh-100 my-5 p-4 border-0 mx-auto bg-white shadow-sm" style="width:210mm;min-width:210mm"><div class="overflow-hidden"><span class="border-bottom border-right float-left" style="width:1.5rem;height:1.5rem"></span><span class="border-left border-bottom float-right" style="width:1.5rem;height:1.5rem"></span></div><div class="card px-4 py-4 border-0 markdown-body editormd-preview-container custom-font"></div></div></div>'
            ),
        };

        this.init();
    }

    Reader.prototype = {
        init: function () {
            this.initStyle();
            this.initEvents();
        },
        initStyle: function () {
            /* 倒序加载DOM */
            var $hTagDoms = this.$domData.find("h1,h2,h3,h4,h5,h6");

            for (var i = 0; i < $hTagDoms.length; i++) {
                var currentItem = $hTagDoms[i],
                    $currentItem = $(currentItem);

                /* 给文档中的H标签添加锚点 */
                $currentItem.attr("id", $currentItem.text());

                if (currentItem.tagName == "H1") {
                    var $nextLevelItem = $(
                        '<a class="nav-link text-black-50 py-3 text-left px-4 rounded-sm small custom-link custom-light-border" data-level="1" href="#' +
                        $currentItem.text() +
                        '">' +
                        $currentItem.text() +
                        "</a>"
                    );
                    this.components.$leftPanelNavContainer.append($nextLevelItem);
                } else this.generateChildLevel(currentItem);
            }

            /* 向左侧面板添加内容 */
            this.components.$leftPanel
                .append(this.components.$leftPanelTopBar)
                .append(this.components.$leftPanelCover);

            this.components.$leftPanelCover.after(
                this.components.$leftPanelNavContainer
            );
            this.components.$container.children().append(this.components.$rightPanel);
            this.components.$container.children().append(this.components.$leftPanel);

            this.components.$leftPanelNavContainer
                .children()
                .first()
                .css("background", "#a2c9f9");


            /* 向右侧面板添加内容 ,初始加载时只显示第一个H1节点及其字节点对应的内容*/
            /* 使用jquery容易弄混子代和同级的查询，下面这行代码实现了一种‘区间查询的效果’ */
            // var firstHTag = this.$domData.find("h1").first().prop("outerHTML");
            // var $firstHTagContent = this.$domData.find("h1").first().nextUntil("h1");
            // var firstHTagContentString = "";

            /*一个常见的问题，在对一个jquery对象集合进行遍历时，遍历的单个元素需要使用$包裹 */
            // for (var i = 0; i < $firstHTagContent.length; i++) {
            //     firstHTagContentString += $($firstHTagContent[i]).prop("outerHTML");
            // }
            /* 在没有添加到dom之前，貌似不可以对其调用after方法 */
            // var $firstHTagTotalContent = $firstHTag.after($firstHTagContent);

            // var firstHTagTotalContent = firstHTag + firstHTagContentString;

            // this.components.$rightPanel.find("div.card").html(firstHTagTotalContent);
            // this.components.$rightPanel
            //     .find("div.card")
            //     .parent()
            //     .append(
            //         '<div class="overflow-hidden"><span class="border-top border-right float-left" style="width:1.5rem;height:1.5rem"></span><span class="border-left border-top float-right" style="width:1.5rem;height:1.5rem"></span></div>'
            //     );

            this.components.$container.children().append(this.components.$rightPanel);

            this.$ele.append(this.components.$container);
        },
        initEvents: function () {

            this.components.$leftPanel
                .find("a")
                .on("click", $.proxy(this.changeNavBg, this));

            this.components.$leftPanelNavContainer
                .children()
                .on("click", $.proxy(this.showOrHideChildLevel, this));

            this.components.$leftPanelNavContainer
                .find("a")
                .on("click", $.proxy(this.showLevelContent, this));

            /* 默认加载第一个标题及其下面的内容，不论这个标题是几级标题 */
            this.components.$leftPanelNavContainer.find("a").first().click();
        },
        showOrHideChildLevel: function (e) {
            var $currentLevel = $(e.target);
            $currentLevel.children("i").toggleClass("fa-angle-right fa-angle-down");

            // this.components.$rightPanel
            //   .find("iframe")
            //   .attr("src", "./" + $currentLevel.text().trim() + ".html");

            if ($currentLevel.attr("isshow") == "true") {
                this.hideChildrenLevel($currentLevel);
            } else {
                this.showChildrenLevel($currentLevel);
            }
        },
        showChildrenLevel: function (selectedLevel) {
            var $selectedLevel = selectedLevel;
            $selectedLevel.attr("isshow", "true");
            $selectedLevel.next().slideDown();
        },
        hideChildrenLevel: function (selectedLevel) {
            var $selectedLevel = selectedLevel;
            $selectedLevel.attr("isshow", "false");
            $selectedLevel.next("div").slideUp();
        },
        changeNavBg: function (e) {
            var $targetItem = $(e.target);
            this.components.$leftPanel.find("a").removeAttr("style");
            $targetItem.css("background", "rgb(162, 201, 249)");
        },
        generateChildLevel: function (tagItem) {
            var nextLevelNum = tagItem.tagName[1],
                parentLevelNum = nextLevelNum - 1;

            var $nextLevelItem = $(
                '<a class="nav-link py-2 text-left px-4 small rounded-sm custom-purple-font custom-light-border custom-small-font custom-link" href="#' +
                $(tagItem).text() +
                '" data-level="' +
                nextLevelNum +
                '">' +
                "&nbsp;&nbsp;".repeat(parentLevelNum) +
                $(tagItem).text() +
                "</a>"
            );

            /* last是对当前选择器选中的dom集合进行过滤，而不是从当前jquery对象的子元素中进行过滤 */
            var $parentLevel = null;
            if (parentLevelNum == 1) {
                $parentLevel = this.components.$leftPanelNavContainer
                    .children("a[data-level='" + parentLevelNum + "']")
                    .last();
            } else {
                $parentLevel = this.components.$leftPanelNavContainer
                    .find("a[data-level='" + parentLevelNum + "']")
                    .last();
            }


            /* 如果没有找到父level */
            if ($parentLevel.length == 0) {
                this.components.$leftPanelNavContainer.append($nextLevelItem);
            }
            /* 找到了父level但是父level还没有子level */
            else if ($parentLevel.children("i").length == 0) {
                $parentLevel.append(
                    '&nbsp;&nbsp;<i class="fa fa-angle-right text-black-50" aria-hidden="true"></i>'
                );
                var $nextLevelContainer = $(
                    "<div class='rounded-sm' style='background-color:rgb(238 242 245)'></div>"
                );
                $nextLevelContainer.append($nextLevelItem);
                $nextLevelContainer.hide();
                $parentLevel.after($nextLevelContainer);
            }
            /* 找到了父level同时父level已经有了子level */
            else {
                $parentLevel.next().append($nextLevelItem);
            }
        },
        /* 在左侧导航栏点击不同的一级链接时在右侧只显示该一级标题下的内容 */
        showLevelContent: function (e) {

            var $targetLevel = $(e.target);
            var currHTagTitle = $targetLevel.text().trim();

            /* 在文章内容中定位到我们对应的HTag,这里的find选择器待优化 */
            var $seletedHTag;
            var allHTags = this.$domData.find("h1,h2,h3,h4,h5,h6");
            for (var i = 0; i < allHTags.length; i++) {
                var $tagItem = $(allHTags[i]);
                if ($tagItem.text() === currHTagTitle) {
                    $seletedHTag = $tagItem;
                }
            }

            /* 获取选中的H标签的HTML */
            var selectedHTagHtml = $seletedHTag.prop("outerHTML");

            /* 获取选中的H标签后面的正文内容的HTML */
            var $selectedHTagContent = $seletedHTag.nextUntil("h1,h2,h3,h4,h5,h6");
            var selectedHTagContentString = "";
            for (var i = 0; i < $selectedHTagContent.length; i++) {
                selectedHTagContentString += $($selectedHTagContent[i]).prop("outerHTML");
            }
            /* 将选中的H标签及其对应的正文内容进行合并 */
            var selectedHTagTotalContent = selectedHTagHtml + selectedHTagContentString;

            this.components.$rightPanel.find("div.card").html(selectedHTagTotalContent);

            /* 如果该标题下没有内容，则直接定位到下一个有内容的标题 */
            if (selectedHTagContentString == "") {
                /* 如果其包含子标题，则定位到其第一个子标题 */
                if ($targetLevel.next()[0].tagName.toLowerCase() == "div") {
                    $targetLevel.next().find("a").first().click();
                } else {
                    /* 如果不包含子标题，则定位到其下一个相邻的兄弟标题 */
                    $targetLevel.next().click();
                }
            }
            this.components.$rightPanel.find("div.custom-card").parent().scrollTop(0);

            /* 确定右侧panel底部定位器的数量，如果已经存在，则不必重复添加 */
            var bottomLocatorNum = this.components.$rightPanel.find(
                "span.border-top.border-right"
            ).length;
            if (bottomLocatorNum === 0) {
                this.components.$rightPanel
                    .find("div.card")
                    .parent()
                    .append(
                        '<div class="overflow-hidden"><span class="border-top border-right float-left" style="width:1.5rem;height:1.5rem"></span><span class="border-left border-top float-right" style="width:1.5rem;height:1.5rem"></span></div>'
                    );
            }
        },
        generateCopyButton: function (htmlContent) {

        }
    };

    $.fn.Reader = function (options) {
        return this.each(function () {
            new Reader(this, options);
        });
    };
})(jQuery, window, document);
