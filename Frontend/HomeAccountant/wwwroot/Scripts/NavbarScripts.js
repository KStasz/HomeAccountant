var navbar;

function InitializeNavbar(navbarId) {
    const menuToggle = document.getElementById(navbarId);
    navbar = bootstrap.Collapse.getOrCreateInstance(menuToggle, { toggle: false });
}

function TryToggleNavbar() {
    if (navbar._isShown()) {
        navbar.toggle();
    }
}