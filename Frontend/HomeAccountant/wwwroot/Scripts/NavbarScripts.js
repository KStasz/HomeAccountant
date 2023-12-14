function CollapseNavabar(navbarId) {
    const menuToggle = document.getElementById(navbarId);
    const navbarCollapse = bootstrap.Collapse.getOrCreateInstance(menuToggle)

    navbarCollapse.toggle();
}