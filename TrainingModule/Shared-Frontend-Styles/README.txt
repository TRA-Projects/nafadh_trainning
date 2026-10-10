Shared Frontend Styles — JobseekerModule Handoff Package
=========================================================

These are the global and reusable frontend styles copied from JobseekerModule
for use in another Angular (or web) project.

CONTENTS
--------
- public/css/style.css       — Main Vristo/Tailwind prebuilt theme (primary UI)
- public/css/animate.css     — Animation utilities
- src/styles.scss            — App-wide global overrides (RTL, icons, modals, toasts, etc.)
- src/app/core/ui/           — Reusable SCSS for form pages, tabs, and paged lists
- src/app/layout/main-layout — Main layout / sidebar / async overlay styles
- public/images/general/     — SVG icons used by icon action buttons and similar UI

REQUIRED NPM PACKAGES
---------------------
This project also requires sweetalert2 and quill (their CSS is imported in styles.scss).

Install them using:

  npm install sweetalert2 quill

MAIN THEME FILES
----------------
The two most important files are:
  1. public/css/style.css  — Link in index.html or import globally
  2. src/styles.scss       — Register in angular.json "styles" array

Also link public/css/animate.css in index.html if you use those animations.

RTL
---
Right-to-left layout uses dir="rtl" on the <html> element.
Left-to-right uses dir="ltr".
Global RTL rules live in src/styles.scss.

DARK MODE
---------
Dark mode uses the "dark" class on <html> or an ancestor element.
Many shared SCSS files use :host-context(.dark) for dark variants.

INTEGRATION NOTES
-----------------
- Load the Nunito font (Google Fonts) if you want typography to match JobseekerModule.
- styles.scss imports sweetalert2 and quill CSS from node_modules; both packages must be installed.
- Reusable partials (form-page-shell, paged-list, etc.) are meant to be @imported from Angular
  component SCSS or copied into your project with the same relative paths.
