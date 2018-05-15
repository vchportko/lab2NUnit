$(document).ready(function () {
    /*scrollbar*/
    if ($('.js-scrollbar').length) {
        $('.js-scrollbar').perfectScrollbar();
    }

    /*for single page*/
    if ($('.js-floating-block').length) {
        let mainContainer = $(document)
        let floatingContainer = $('.js-floating-block')
        let currTop = parseInt($('.js-floating-block').css('top'))
        let floatingContainerTop = floatingContainer.offset().top

        $(window).scroll(function () {
            let currScroll = mainContainer.scrollTop()
            if (currScroll >= floatingContainerTop) {
                $('.js-floating-block').css('top', currScroll - floatingContainerTop)
            }
            else {
                $('.js-floating-block').css('top', currTop)
            }
        })
    }

    /*for modals*/
    $('.js-open-modal').click(function () {
        let modalId = $(this).data('modal')
        $(modalId).addClass('visible')
        $('body').addClass('modal-open')
    })

    $('.js-close-modal').click(function () {
        $('.modal').removeClass('visible')
        $('body').removeClass('modal-open')
    })

    /*for checkbox*/
    $('.js-checkbox').click(function () {
        let currCheckbox = $(this)
        if (currCheckbox.hasClass('-checked')) {
            currCheckbox.removeClass('-checked')
        } else {
            currCheckbox.addClass('-checked')
        }
    })

    $('.js-search-button').click(function () {
        let inputVal = $(this).closest('.js-search').find('#js-search-input').val().split(',')
        let hasRes = false;
        if (inputVal.length == 1 && inputVal[0] != "") {
            let arr = []
            $('.js-search-item').each(function (item) {
                let obj = {
                    container: $('.js-search-item').eq(item),
                    recipeName: $('.js-search-item').eq(item).find('.js-item-name').text().toLowerCase()
                }
                arr.push(obj)
            })

            for (let i = 0; i < arr.length; i++) {
                if (arr[i].recipeName.includes(inputVal[0])) {
                    arr[i].container.show()
                    hasRes = true
                }
                else {
                    arr[i].container.hide()
                }
            }
        }
        if ((inputVal.length != 1 || inputVal[0] != "") && hasRes == false) {
            let arr = []
            $('.js-search-item').each(function (item) {
                let obj = {
                    container: $('.js-search-item').eq(item),
                    ingredients: $('.js-search-item').eq(item).attr('data-ingredients').split(',')
                }
                arr.push(obj)
            })
            for (let i = 0; i < arr.length; i++) {
                if (intersect(arr[i].ingredients, inputVal).length == inputVal.length || intersect(arr[i].ingredients, inputVal).length == arr.length) {
                    arr[i].container.show()
                }
                else {
                    arr[i].container.hide()
                }
            }
        }
    })

    function intersect(arr1, arr2) {
        var arr3 = [];
        for (i = 0; i < arr1.length; i++) {
            for (j = 0; j < arr2.length; j++) {
                if (arr1[i].includes(arr2[j])) {
                    arr3.push(arr1[i]);
                }
            }
        }
        return arr3;
    }
})
