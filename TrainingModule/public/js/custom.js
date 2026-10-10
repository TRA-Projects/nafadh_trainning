/// <reference path="perfect-scrollbar.min.js" />
(function () {

    const $themeConfig = {
        locale: 'ar', // en, da, de, el, es, fr, hu, it, ja, pl, pt, ru, sv, tr, zh
        flag:'ar',
        theme: 'light', // light, dark, system
        menu: 'vertical', // vertical, collapsible-vertical, horizontal
        layout: 'full', // full, boxed-layout
        rtlClass: 'rtl', // rtl, ltr
        animation: '', // animate__fadeIn, animate__fadeInDown, animate__fadeInUp, animate__fadeInLeft, animate__fadeInRight, animate__slideInDown, animate__slideInLeft, animate__slideInRight, animate__zoomIn
        navbar: 'navbar-sticky', // navbar-sticky, navbar-floating, navbar-static
        semidark: false,
    };

   

    window.addEventListener('load', function () {
        // screen loader
        const screen_loader = document.getElementsByClassName('screen_loader');
        if (screen_loader?.length) {
            screen_loader[0].classList.add('animate__fadeOut');
            setTimeout(() => {
                document.body.removeChild(screen_loader[0]);
            }, 200);
        }

        //remove initail date range picker from screen, bug fix Jickson
        const range_loader = document.getElementsByClassName('daterangepicker');
        if (range_loader?.length) {
            range_loader[0].classList.remove('show-calendar');
        }

        // set rtl layout
        Alpine.store('app').setRTLLayout();
    });

    // set current year in footer
    const yearEle = document.querySelector('#footer-year');
    if (yearEle) {
        yearEle.innerHTML = new Date().getFullYear();
    }

    // perfect scrollbar
    const initPerfectScrollbar = () => {
        const container = document.querySelectorAll('.perfect-scrollbar');
        for (let i = 0; i < container.length; i++) {
            new PerfectScrollbar(container[i], {
                wheelPropagation: true,
                // suppressScrollX: true,
            });
        }
    };
    initPerfectScrollbar();

    document.addEventListener('alpine:init', () => {
        Alpine.data('collapse', () => ({
            collapse: false,

            collapseSidebar() {
                this.collapse = !this.collapse;
            },
        }));

        Alpine.data('dropdown', (initialOpenState = false) => ({
            open: initialOpenState,

            toggle() {
                this.open = !this.open;
            },
        }));
        Alpine.data('modal', (initialOpenState = false) => ({
            open: initialOpenState,

            toggle() {
                this.open = !this.open;
            },
        }));

        // Magic: $tooltip
        Alpine.magic('tooltip', (el) => (message, placement) => {
            let instance = tippy(el, {
                content: message,
                trigger: 'manual',
                placement: placement || undefined,
                allowHTML: true,
            });

            instance.show();
        });

        Alpine.directive('dynamictooltip', (el, { expression }, { evaluate }) => {
            let string = evaluate(expression);
            tippy(el, {
                content: string.charAt(0).toUpperCase() + string.slice(1),
            });
        });

        // Directive: x-tooltip
        Alpine.directive('tooltip', (el, { expression }) => {
            tippy(el, {
                content: expression,
                placement: el.getAttribute('data-placement') || undefined,
                allowHTML: true,
                delay: el.getAttribute('data-delay') || 0,
                animation: el.getAttribute('data-animation') || 'fade',
                theme: el.getAttribute('data-theme') || '',
            });
        });

        // Magic: $popovers
        Alpine.magic('popovers', (el) => (message, placement) => {
            let instance = tippy(el, {
                content: message,
                placement: placement || undefined,
                interactive: true,
                allowHTML: true,
                // hideOnClick: el.getAttribute("data-dismissable") ? true : "toggle",
                delay: el.getAttribute('data-delay') || 0,
                animation: el.getAttribute('data-animation') || 'fade',
                theme: el.getAttribute('data-theme') || '',
                trigger: el.getAttribute('data-trigger') || 'click',
            });

            instance.show();
        });

        // main - custom functions
        Alpine.data('main', (value) => ({}));
        
        if (!Alpine.store('app')) {
            Alpine.store('app', {
                // theme
                languages: [
                    {
                        id: 3,
                        key: 'English',
                        flag: 'en',
                        value: 'en',
                    },
                    {
                        id: 16,
                        key: 'عربي',
                        flag: 'om',
                        value: 'ar',
                    },
                ],
                registeration: null,
                setRegisteration(val) {
                    console.log('comsing here');
                    this.registeration = val;
                },
                theme: Alpine.$persist($themeConfig.theme),
                isDarkMode: Alpine.$persist(false),
                toggleTheme(val) {
                    if (!val) {
                        val = this.theme || $themeConfig.theme; // light|dark|system
                    }

                    this.theme = val;

                    if (this.theme == 'light') {
                        this.isDarkMode = false;
                    } else if (this.theme == 'dark') {
                        this.isDarkMode = true;
                    } else if (this.theme == 'system') {
                        if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                            this.isDarkMode = true;
                        } else {
                            this.isDarkMode = false;
                        }
                    }
                },

                // navigation menu
                menu: Alpine.$persist($themeConfig.menu),
                toggleMenu(val) {
                    if (!val) {
                        val = this.menu || $themeConfig.menu; // vertical, collapsible-vertical, horizontal
                    }
                    this.sidebar = false; // reset sidebar state
                    this.menu = val;
                },

                // layout
                layout: Alpine.$persist($themeConfig.layout),
                toggleLayout(val) {
                    if (!val) {
                        val = this.layout || $themeConfig.layout; // full, boxed-layout
                    }

                    this.layout = val;
                },

                // rtl support
                rtlClass: Alpine.$persist($themeConfig.rtlClass),
                toggleRTL(val) {
                    if (!val) {
                        val = this.rtlClass || $themeConfig.rtlClass; // rtl, ltr
                    }

                    this.rtlClass = val;
                    this.setRTLLayout();
                },

                setRTLLayout() {
                    document.querySelector('html').setAttribute('dir', this.rtlClass || $themeConfig.rtlClass);
                },

                // animation
                animation: Alpine.$persist($themeConfig.animation),
                toggleAnimation(val) {
                    if (!val) {
                        val = this.animation || $themeConfig.animation; // animate__fadeIn, animate__fadeInDown, animate__fadeInLeft, animate__fadeInRight
                    }
                    val = val?.trim();

                    this.animation = val;
                },

                // navbar type
                navbar: Alpine.$persist($themeConfig.navbar),
                toggleNavbar(val) {
                    if (!val) {
                        val = this.navbar || $themeConfig.navbar; // navbar-sticky, navbar-floating, navbar-static
                    }

                    this.navbar = val;
                },

                
                // semidark
                semidark: Alpine.$persist($themeConfig.semidark),
                toggleSemidark(val) {
                    if (!val) {
                        val = this.semidark || $themeConfig.semidark;
                    }

                    this.semidark = val;
                },

                // multi language
                flag: Alpine.$persist($themeConfig.flag),

                chatHubURL: 'http://localhost:58888/chathub',
                articleImagePath: 'http://localhost:58888/ArticleImages/',
                ImagePath: 'http://localhost:58888/Images/',

                locale: Alpine.$persist($themeConfig.locale),
                async toggleLocale(val) {
                    if (!val) {
                        val = this.locale || $themeConfig.locale;
                    }
                    const currentUrl = new URL(window.location.href);
                    const pathParts = currentUrl.pathname.split('/');

                    if (pathParts.length > 1) {
                        pathParts[1] = val; // Replace culture part
                    }

                    const newUrl = pathParts.join('/') + currentUrl.search + currentUrl.hash;
                    console.log(newUrl);
                    
                    
                    this.locale = val;
                    if (this.locale?.toLowerCase() === 'ar') {
                        this.toggleRTL('rtl');
                    } else {
                        this.toggleRTL('ltr');
                    }

                    window.location.href = newUrl;
                },

                // sidebar
                sidebar: false,
                toggleSidebar() {
                    this.sidebar = !this.sidebar;
                },

                gotoHome() {
                    window.location.href = '/';
                }   

            });
        }

    });
})();
