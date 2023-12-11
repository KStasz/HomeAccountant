let Modals = new Map();
function HideModal() {
    var modal = bootstrap.Modal.getInstance('#authenticationModal');
    modal.hide();
}

function Initialize(modalId) {
    var modalItem = document.getElementById(modalId);

    var modal = bootstrap.Modal.getOrCreateInstance(modalItem);

    if (Modals === null
        || !Modals.has(modalId)) {
        Modals.set(modalId, modal);
    }
}

function ShowModalWithIdentifier(modalId) {

    var modal = Modals.get(modalId);

    if (modal !== undefined || modal !== null) {
        modal.show();
    }
}

function HideModalWithIdentifier(modalId) {

    var modal = Modals.get(modalId);

    if (modal !== null) {
        modal.hide();
    }
}