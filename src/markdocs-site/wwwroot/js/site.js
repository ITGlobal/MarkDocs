$(function () {
    var galleryOpts = {
        arrowNavigation: true
    };

    var luminousOpts = {
        closeOnScroll: true,
        injectBaseStyles: false
    };

    $('a[data-thumbnail]').each(function () {
        var luminous = new Luminous(this, luminousOpts);
    });
    $('div[data-gallery]').each(function () {

        var luminousGallery = new LuminousGallery($('a[data-gallery-item]', this), galleryOpts, luminousOpts);
    });

    if (window.location.hash) {
        $(window.location.hash).css('background-color', '#ffc107').parent().animate({
            backgroundColor: '#ffffff'
        },
            2000);

        $('*[name=' + window.location.hash.substr(1) + ']').parent().css('background-color', '#ffc107').animate({
            backgroundColor: '#ffffff'
        },
            2000);
    }

    var activeSidebarItem = $('.sidebar .active')[0];
    if (activeSidebarItem) {
        activeSidebarItem.scrollIntoView();
    }
});

function createSidebar(element, sidebarModel, activeId) {
    var nodeMap = {};
    var parentMap = {};

    function mapPageTreeNode(n, skipNestedPages, parent) {
        var result = {
            id: n.id,
            text: (n.shortTitle || n.title),
            isHyperlink: n.isHyperlink,
            href: n.id,
            state: {
                expanded: false
            }
        };
        if (!skipNestedPages) {
            if (n.pages.length > 0) {
                result.nodes = [];
                for (var i = 0; i < n.pages.length; i++) {
                    result.nodes.push(mapPageTreeNode(n.pages[i], false, result));
                }
            }
        }
        nodeMap[result.id] = result;
        if (parent) {
            parentMap[result.id] = parent;
        }
        return result;
    }

    var root = sidebarModel;
    var nodes = [];
    nodes.push(mapPageTreeNode(root, true, null));
    for (var i = 0; i < root.pages.length; i++) {
        nodes.push(mapPageTreeNode(root.pages[i], false, null));
    }

    var activeNode = nodeMap[activeId];
    if (activeNode) {
        activeNode.state.selected = true;

        while (activeNode) {
            activeNode.state.expanded = true;
            activeNode = parentMap[activeNode.id];
        }
    }

    element.treeview({
        data: nodes,
        levels: 0,
        enableLinks: true,
        showBorder: false,
        color: '#007bff',
        selectedColor: '#ffffff',
        selectedBackColor: '#007bff',
        expandIcon: 'fa fa-folder fa-fw',
        collapseIcon: 'fa fa-folder-open fa-fw',
        emptyIcon: 'fa fa-caret-right fa-fw',
        onNodeSelected: function (event, data) {
            if (data.href) {
                window.location = window.location.protocol + '//' + window.location.host + data.href;
            }
        }
    });
}